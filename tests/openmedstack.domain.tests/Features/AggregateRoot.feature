Feature: Aggregate Root
Aggregate root functionality verification

    Background:
        Given a started service

    Scenario: Default event handler registration
        Given an aggregate root
        When applying a known event
        Then the event is handled

    Scenario: When executing action, then read store is updated.
        When performing action
        And processing is finished
        Then data store is updated 1 times

    Scenario: When executing action twice, then read store is updated twice.
        When performing action
        And performing action
        And processing is finished
        Then data store is updated 2 times
