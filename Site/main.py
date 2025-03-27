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

@app.route('/logout', methods=['GET'])
def logout():
    #Using session here allows us to clear all data
    #This means that we don't have a hard logout yet.
    #A user logged in will always be until logged out manually.
    flask.session.clear()
    return flask.redirect('/')

def str_to_bool(string):
    if string == "True":
        return True
    return False

def main():
    try:
        global startUp
        startUp = True
        app.run(host='0.0.0.0', port = 8000, debug=True)
    except Exception as ex:
        print(ex, file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
