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
db_pwd = os.getenv("DB_PASS", "")

def query_leaderboard(level):
    conn = psycopg2.connect(database = "Primary DB", 
                            user = "postgres", 
                            host= 'localhost',
                            password = db_pwd,
                            port = 5432)
    with conn.cursor() as curr:
        query = 'SELECT netid, deaths, "time" FROM runs'
        query += ' WHERE lvl=%s ORDER BY "time" ASC, deaths ASC' % level
        curr.execute(query)
        rows = curr.fetchall()
    conn.commit()
    conn.close()

    return rows

def insert_db(params):
    time = datetime.datetime.now()
    time.strftime('%Y-%m-%d %H:%M:%S')
    with psycopg2.connect(database = "Primary DB", 
                            user = "postgres", 
                            host= 'localhost',
                            password = db_pwd,
                            port = 5432) as conn:
        with conn.cursor() as curr:
            curr.execute('INSERT INTO runs(run_id, netid, lvl, deaths, "time", "timestamp") ' +
                            "VALUES('%s', '%s', '%s', '%s', '%s', '%s')" % (
                            params['run_id'],
                            params['netid'],
                            params['lvl'],
                            params['deaths'],
                            params['time'],
                            time,
                            ))
        conn.commit()

#For testing purposes
def main():
    query_leaderboard()
    time = datetime.datetime.now()
    time.strftime('%Y-%m-%d %H:%M:%S')
    params = {
        'run_id': 1,
        'netid': 'sh3735',
        'lvl': 1,
        'deaths': 3,
        'time': 21.4,
        'timestamp': time
    }
    insert_db(params)
    query_leaderboard()

if __name__ == '__main__':
    main()