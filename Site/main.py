#!/usr/bin/env python

#-----------------------------------------------------------------------
# main.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import sys
import flask
import auth
import argparse

from db_tools import query_leaderboard, insert_db, get_next_run_id
from load import app
import utils

#-----------------------------------------------------------------------

@app.route('/', methods=['GET'])
def home():
    #getting log in information
    logged_in = flask.session.get('logged_in', False) 
    username = flask.session.get('username', None)
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
    return flask.redirect('/')

@app.route('/gametest', methods=['GET'])
def gametest():
    rendered = flask.render_template('game.html')
    response = flask.make_response(rendered)
    response.headers['Content-Encoding'] = 'brotli'
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

    params = {
        'run_id': run_id,
        'netid' : netid,
        'lvl': int(level[5:]),
        'deaths' : deaths,
        'time' : time,
    }

    if (netid is not None):
        insert_db(params)
    else:
        print("Did not go through")

    return flask.redirect('/leaderboard-menu')
    

@app.route('/logout', methods=['GET'])
def logout():
    #Using session here allows us to clear all data
    #This means that we don't have a hard logout yet.
    #A user logged in will always be until logged out manually.
    flask.session.clear()
    return flask.redirect('/')

@app.route('/leaderboard-menu', methods=['GET'])
def leader_menu():
    logged_in = flask.session.get('logged_in', False)
    username = flask.session.get('username', None) 
    return flask.render_template('leadermenu.html', log=logged_in,
                                 username = username)

@app.route('/leaderboard', methods=['GET'])
def leaderboard():
    lvl = flask.request.args.get('lvl', None)
    pg = int(flask.request.args.get('pg', 1))

    if lvl == None:
        return flask.redirect('/')
    table_info = query_leaderboard(lvl)

    limit = 10 # Number of rows on table at once
    if pg * limit > len(table_info):
        pg = len(table_info) // limit
        pg += 1 if len(table_info) % limit != 0 else 0

    return flask.render_template('leaderboard.html', table = table_info,
                                lvl = lvl, pg = pg, limit = limit)

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
                        metavar="port", type=int, default=8000)

    args = parser.parse_args()

    try:
        app.run(host='localhost', port = args.port, debug=False)
    except Exception as ex:
        print(ex, file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
