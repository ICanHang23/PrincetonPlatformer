#！/usr/bin/env python

import flask
import dotenv
import os

dotenv.load_dotenv()

app =flask.Flask('main', template_folder = 'templates')
app.config.update(
    SECRET_KEY = os.environ['APP_SECRET_KEY']
) 