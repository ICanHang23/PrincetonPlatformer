#!/usr/bin/env python

#-----------------------------------------------------------------------
# main.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import flask

from load import app
import utils

#-----------------------------------------------------------------------

@app.route("/error/<code>")
def error(code, message = None):
    if message is not None:
        error_title = "Error %s" % code
        error_msg = message
    else:
        error_title = "Error 500"
        error_msg = "Internal Server Error"
        print(code)
    logged_in = flask.session.get('logged_in', False)
    username =  flask.session.get('username', None)

    ref = utils.get_last_page()
    return flask.render_template('error.html', error_title = error_title,
                                 error_msg = error_msg, username = username, 
                                 log = logged_in, ref = ref)

@app.errorhandler(400)
def err400(error_given):
    error_given = str(error_given)
    index = error_given.find(':') + 1
    error("400 - Bad Request", error_given[index:])

@app.errorhandler(403)
def err403(error_given):
    error_given = str(error_given)
    index = error_given.find(':') + 1
    error("403 - Forbidden", error_given[index:])

@app.errorhandler(404)
def err404(error_given):
    error_given = str(error_given)
    index = error_given.find(':') + 1
    return error("404 - Page Not Found", error_given[index:])