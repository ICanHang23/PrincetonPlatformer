#!/usr/bin/env python

#-----------------------------------------------------------------------
# main.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import flask

#-----------------------------------------------------------------------

def str_to_bool(string):
    if string == "True":
        return True
    return False

def get_form_params():
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
    return params
