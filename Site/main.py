#!/usr/bin/env python

#-----------------------------------------------------------------------
# main.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import sys
import flask
import auth
import argparse
import urllib.parse
import dotenv
import os
import json

from db_tools import (
    query_leaderboard, 
    insert_db, 
    get_next_run_id, 
    query_times,
    get_ghost_info,
    get_run_info
)
from load import app
import utils

#-----------------------------------------------------------------------

dotenv.load_dotenv()
debug = os.environ.get('DEBUG')
debug = utils.str_to_bool(debug) if debug != None else False

#-----------------------------------------------------------------------

@app.route('/', methods=['GET'])
def home():
    #getting log in information
    logged_in = flask.session.get('logged_in', False) 
    username = flask.session.get('username', None)
    utils.set_last_page('/')
    return flask.render_template('index.html', log=logged_in,
                                  username = username)

@app.route('/login', methods=['GET'])
def login():
    user_info = auth.authenticate()
    #making sure that user info is gathered correctly 
    if isinstance(user_info, flask.Response):
        return user_info
    
    #get username
    username = user_info.get('user')
    #checking if username is not None
    if username:
        # creating a session variable named logged in
        flask.session['logged_in'] = True
        flask.session['username'] = username
    
    #going back to the index
    utils.set_last_page('/')
    return flask.redirect('/')

@app.route('/game', methods=['GET'])
def game():
    logged_in = flask.session.get('logged_in', False) 
    rendered = flask.render_template('game.html', log=logged_in)
    response = flask.make_response(rendered)
    response.headers['Content-Encoding'] = 'brotli'
    utils.set_last_page('/game')
    return response

# I'm pretty sure post is right here to receive data?
@app.route('/receivescore', methods = ['POST'])
def receivescore():
    data = flask.request.get_json()
    netid = flask.session.get('username', None)
    run_id = get_next_run_id(netid) if netid is not None else 1
    time = data.get('time')
    level = data.get('level')
    deaths = data.get('deaths')
    inputs = data.get('input')

    params = {
        'run_id': run_id,
        'netid' : netid,
        'lvl': int(level[5:]),
        'deaths' : deaths,
        'time' : round(time, 2),
        'inputs' : json.dumps(inputs)
    }

    print("Game inputs")
    print(params['inputs'])
    print(params)

    if (netid is not None):
        insert_db(params)

    return flask.redirect('/leaderboard-menu')
    

@app.route('/logout', methods=['GET'])
def logout():
    #Using session here allows us to clear all data
    #This means that we don't have a hard logout yet.
    #A user logged in will always be until logged out manually.
    flask.session.clear()
    return flask.redirect('/')

@app.route('/signout', methods=['GET'])
def signout():
    flask.session.clear()
    cas_logout_url = 'https://fed.princeton.edu/cas/logout'
    return_url = flask.url_for('home', _external=True)
    logout_redirect_url = f"{cas_logout_url}?service={urllib.parse.quote(return_url)}"
    return flask.redirect(logout_redirect_url)

# this is a endpoint to send information to the game. for now we just send netID but in the future we can send more
@app.route('/getprofile', methods=['GET'])
def get_profile():
    username = flask.session.get('username', None)
    
    if not username:
        return flask.jsonify({"username": "Guest"})

    # Pull from session, or eventually a database
    return flask.jsonify({
        "username": username,
    })

@app.route('/leaderboard-menu', methods=['GET'])
def leader_menu():
    logged_in = flask.session.get('logged_in', False)
    username = flask.session.get('username', None)
    utils.set_last_page('/leaderboard-menu')
    return flask.render_template('leadermenu.html', log=logged_in,
                                 username = username)

@app.route('/leaderboard', methods=['GET'])
def leaderboard():
    logged_in = flask.session.get('logged_in', False) 
    lvl = flask.request.args.get('lvl', None)
    pg = int(flask.request.args.get('pg', 1))

    if lvl == None:
        return flask.redirect('/')
    table_info = query_leaderboard(lvl)

    limit = 10 # Number of rows on table at once
    if pg * limit > len(table_info):
        pg = len(table_info) // limit
        pg += 1 if len(table_info) % limit != 0 else 0

    utils.set_last_page('/leaderboard?lvl=%s&pg=%s' % (lvl, pg))
    return flask.render_template('leaderboard.html', table = table_info,
                                lvl = lvl, pg = pg, limit = limit, log=logged_in)

@app.route('/times/<user>')
def times(user):
    logged_in = flask.session.get('logged_in', False)
    pg = int(flask.request.args.get('pg', 1))

    table_info = query_times(user)

    limit = 10 # Number of rows on table at once
    if pg * limit > len(table_info):
        pg = len(table_info) // limit
        pg += 1 if len(table_info) % limit != 0 else 0

    ref = utils.get_last_page()
    return flask.render_template('leaderboard.html', table = table_info,
                                username = user, pg = pg, limit = limit,
                                log=logged_in, ref = ref)

# gets called upon clicking the "watch" button in the leaderboard
# sets cookies and redirects the user to the game page
@app.route('/ghost', methods=['GET'])
def ghost():
    net_id = flask.request.args.get('net_id', "")
    run_id = flask.request.args.get('run', 0)

    flask.session["gho_net"] = net_id
    flask.session["gho_run"] = run_id
    return flask.redirect('/game')

# gets called by the embedded game player when retrieving information
# for ghosts. returns a json or an error status
@app.route("/getghost", methods=['GET'])
def get_ghost():
    net_id = flask.session.get("gho_net", "")
    run_id = flask.session.get("gho_run", "")

    # checks for absent cookies
    if net_id == "" or run_id == "":
        flask.abort(400)
    
    params = {"run_id": run_id, "netid": net_id}

    ghost_db_info = get_ghost_info(params)
    if (len(ghost_db_info) == 0):
        flask.abort(404)
    ghost_json = ghost_db_info[0][0]

    run_db_info = get_run_info(params)
    if (len(get_run_info) == 0):
        flask.abort(404)
    lvl = run_db_info[0][0]
    deaths = run_db_info[0][1]
    time = run_db_info[0][2]

    output = {"inputs": ghost_json, "lvl": lvl,
              "deaths": deaths, "time": time}

    return flask.jsonify(output)


  
@app.route('/insert', methods=['POST'])
def insert():
    params = utils.get_form_params()
    insert_db(params)
    return flask.redirect('/leaderboard-menu')

def main():
    # set up parser
    parser = argparse.ArgumentParser(
                    prog=sys.argv[0],
                    description="The registrar application")

    # add arguments
    parser.add_argument("port", help="""the port at which the
                        server should listen""", nargs='?',
                        metavar="port", type=int, default=5000)

    args = parser.parse_args()

    try:
        app.run(host='localhost', port = args.port, debug=debug)
    except Exception as ex:
        print(ex, file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
