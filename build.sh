#!/bin/sh
TERM=xterm Unity -quit -batchmode -logFile /dev/stdout -projectpath . -executeMethod BuildCommand.BuildAndroid
