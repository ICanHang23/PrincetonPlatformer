#!/usr/bin/env python

#-----------------------------------------------------------------------
# main.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import sys
import flask
import auth
import dotenv

#-----------------------------------------------------------------------

app = flask.Flask(__name__, template_folder='.')
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
        response.set_cookie('log', "False")
        return response
    print('home was run')
    logged_in = str_to_bool(flask.request.cookies.get('log', None))
    return flask.render_template('index.html', log=logged_in)

@app.route('/login', methods=['GET'])
def login():
    user_info = auth.authenticate()
    # print(user_info)
    username = user_info['user']
    #check if username is none
    if (not username):
        rendered = flask.render_template('index.html', log = False)
        response.set_cookie('log', 'False')
    else: 
        rendered = flask.render_template('index.html', log = False)
        response.set_cookie('log', 'True')
    response = flask.make_response(rendered)
    return response

@app.route('/logout', methods=['GET'])
def logout():
    redirect = flask.redirect('/')
    response = flask.make_response(redirect)
    response.set_cookie('log', 'False')
    return response

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