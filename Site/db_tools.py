#!/usr/bin/env python

#-----------------------------------------------------------------------
# db_tools.py
# Author: Samuel Hilbert
#-----------------------------------------------------------------------

import psycopg2
import dotenv
import os
import datetime

#-----------------------------------------------------------------------


dotenv.load_dotenv()
# You need a password in your .env for this to work
db_user = os.getenv("DB_USER", "")
db_pwd = os.getenv("DB_PASS", "")
db_host = os.getenv("DB_HOST", "")

def query_leaderboard(level):
    conn = psycopg2.connect(database = "Primary DB", 
                            user = db_user, 
                            host= db_host,
                            password = db_pwd,
                            port = 5432)
    with conn.cursor() as curr:
        query = 'SELECT netid, deaths, "time", run_id, has_ghost FROM runs'
        query += ' WHERE lvl=%s ORDER BY "time" ASC, deaths ASC' % level
        curr.execute(query)
        rows = curr.fetchall()
    conn.commit()
    conn.close()

    return rows

def query_times(username):
    conn = psycopg2.connect(database = "Primary DB", 
                            user = db_user, 
                            host= db_host,
                            password = db_pwd,
                            port = 5432)
    with conn.cursor() as curr:
        query = 'SELECT run_id, lvl, deaths, "time", has_ghost FROM runs'
        query += " WHERE netid='%s' ORDER BY run_id ASC" % username
        curr.execute(query)
        rows = curr.fetchall()
    conn.commit()
    conn.close()

    return rows

def get_next_run_id(netid: str):
    with psycopg2.connect(database = "Primary DB", 
                            user = db_user, 
                            host= db_host,
                            password = db_pwd,
                            port = 5432) as conn:
        with conn.cursor() as curr:
            query = "SELECT run_id FROM runs WHERE netid LIKE '%s' " % netid
            query += "ORDER BY run_id DESC"
            curr.execute(query)
            rows = curr.fetchall()
            output = rows[0][0] + 1 if len(rows) != 0 else 1
        conn.commit()

    return output

def get_ghost_info(params):
    with psycopg2.connect(database = "Primary DB", 
                            user = db_user, 
                            host= db_host,
                            password = db_pwd,
                            port = 5432) as conn:
        with conn.cursor() as curr:
            curr.execute("SELECT ghost_info FROM ghosts WHERE netid='%s' AND run_id='%s'" % (
                params['netid'], params['run_id']
            ))
            rows = curr.fetchall()
            output = rows[0][0]
        conn.commit()

    return output

def insert_db(params):
    time = datetime.datetime.now()
    time.strftime('%Y-%m-%d %H:%M:%S')
    with psycopg2.connect(database = "Primary DB", 
                            user = db_user, 
                            host= db_host,
                            password = db_pwd,
                            port = 5432) as conn:
        with conn.cursor() as curr:
            curr.execute('INSERT INTO runs(run_id, netid, lvl, deaths, "time", "timestamp", has_ghost) ' +
                            "VALUES('%s', '%s', '%s', '%s', '%s', '%s', TRUE)" % (
                            params['run_id'],
                            params['netid'],
                            params['lvl'],
                            params['deaths'],
                            params['time'],
                            time,
                            ))
        conn.commit()
        with conn.cursor() as curr:
            curr.execute("INSERT INTO ghosts(run_id, netid, ghost_info) VALUES('%s', '%s', '%s')" % (
                params['run_id'], params['netid'], params['inputs']
            ))
        conn.commit()

#For testing purposes
def main():
    print(get_ghost_info({"run_id": 11, "netid": 'sh3735'}))
    # query_leaderboard()
    # params = {
    #     'run_id': 1,
    #     'netid': 'sh3735',
    #     'lvl': 1,
    #     'deaths': 3,
    #     'time': 21.4,
    # }
    # insert_db(params)
    # query_leaderboard()

if __name__ == '__main__':
    main()