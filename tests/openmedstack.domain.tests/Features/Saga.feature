Feature: Saga
Saga behavior verification

    Background:
        Given a started service

    Scenario: Saga event handling
        When a saga handles an event
        And processing is finished
        Then it is sent on command bus

    Scenario: Transitioning side effects
        When loading a saga
        When transitioning
        Then has uncommitted events

    Scenario: Known event handling
        When loading a saga
        And transitioning
        Then event is handled
