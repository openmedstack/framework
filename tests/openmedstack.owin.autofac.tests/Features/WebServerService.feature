Feature: Web Server Workflow
Verification of web server features

    Background: Web service setup
        Given a web service
        And a client
        And an event subscription

    Scenario: Event publication
        When requesting application root
        Then event is published on bus

        Scenario: When requesting command path then command is sent on bus
            When requesting command path
            Then event is published on bus
