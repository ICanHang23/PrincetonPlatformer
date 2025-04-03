#!/usr/bin/env python

#-----------------------------------------------------------------------
# main.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import sys
import flask
import auth
import dotenv
import os
import argparse

from db_tools import query_leaderboard, insert_db
from load import app

#-----------------------------------------------------------------------

dotenv.load_dotenv()
app.secret_key = os.environ['APP_SECRET_KEY']

#-----------------------------------------------------------------------

@app.route('/', methods=['GET'])
def home():
    global startUp
    if startUp:
        print("Start Up happened")
        startUp = False
        rendered = flask.render_template('index.html', log=False)
        response = flask.make_response(rendered)
        return response
    print('home was run')
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
    time = data.get('time')
    level = data.get('level')
    deaths = data.get('deaths')

    params = {
        'run_id': 45102973,
        'netid' : netid,
        'lvl': int(level[5:]),
        'deaths' : deaths,
        'time' : time,
    }

    insert_db(params)

    return flask.jsonify({'message': 'Time submitted', 
                          'player_name': netid})
    

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
    if lvl == None:
        print("Something went wrong")
        return flask.redirect('/')
    table_info = query_leaderboard(lvl)
    for row in table_info:
        print(row[0])
        print(row[1])
        print(row[2])
    return flask.render_template('leaderboard.html', table = table_info)

@app.route('/insert', methods=['POST'])
def insert():
    run_id = flask.request.form.get('run_id', 0)
    netid = flask.request.form.get('netid', '')
    netid = flask.session.get('username', None) if netid == '' else netid
    lvl = flask.request.form.get('lvl', 1)
    deaths = flask.request.form.get('deaths', 0)
    time = flask.request.form.get('time', 50)
    params = {
        'run_id': run_id,
        'netid': netid,
        'lvl': lvl,
        'deaths': deaths,
        'time': time,
    }
    insert_db(params)
    return flask.redirect('/leaderboard-menu')

def str_to_bool(string):
    if string == "True":
        return True
    return False

def main():
    # set up parser
    parser = argparse.ArgumentParser(
                    prog=sys.argv[0],
                    description="The registrar application")

    # add arguments
    parser.add_argument("port", help="""the port at which the
                        server should listen""",
                        metavar="port", type=int)

    args = parser.parse_args()

    try:
        global startUp
        startUp = True
        app.run(host='localhost', port = args.port, debug=True)
    except Exception as ex:
        print(ex, file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
