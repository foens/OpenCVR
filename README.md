OpenCVR [![Build Status](https://travis-ci.org/foens/OpenCVR.svg?branch=master)](https://travis-ci.org/foens/OpenCVR)
======

# Setup

## Requirements
 * A valid user for the [cvr.dk subscription service](http://datacvr.virk.dk/data/legacy?page=cvr).
 * An exchange email address receiving cvr.dk subscriptions
    * It should be simple to implement a POP3 or IMAP client, if you are not using an exchange service.

## Common setup
 * Change the file OpenCVR.exe.config `<appSettings>` section, to suit your needs.
 * Create a xml file containing secrets:

### Secrets

    <?xml version="1.0" encoding="utf-8" ?>
    <CvrConfiguration
      CvrPassword="<password to cvr user>"
      ExchangeServiceUserName="<exchange username. Example: JohnDoe>"
      ExchangeServicePassword="<exchange password>"
      ExchangeServiceHost="<exchange host. Example: https://mail.example.com/EWS/Exchange.asmx>"
      EmailAddress="<email address. Example: JohnDoe@example.com>">
    </CvrConfiguration> 


## Windows specific setup
 * None so far

## Linux specific setup
How to get up an running on linux:
  * Install sqlite3 on the system
  * Replace Ninject libraries with Mono 4.0 versions from [here](https://teamcity.bbv.ch/viewLog.html?buildId=5525&buildTypeId=bt7&tab=artifacts)
