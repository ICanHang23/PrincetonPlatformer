#!/usr/bin/env python

#-----------------------------------------------------------------------
# main.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import flask
import db_tools

#-----------------------------------------------------------------------

def str_to_bool(string):
    if string == "True":
        return True
    return False

def get_form_params():
    netid = flask.request.form.get('netid', '')
    netid = flask.session.get('username', None) if netid == '' else netid
    run_id = db_tools.get_next_run_id(netid)
    print("Run ID: ", run_id)
    lvl = flask.request.form.get('lvl', 1) or 1
    deaths = flask.request.form.get('deaths', 0) or 0
    time = flask.request.form.get('time', 50) or 50
    params = {
        'run_id': run_id,
        'netid': netid,
        'lvl': lvl,
        'deaths': deaths,
        'time': time,
    }
    return params
