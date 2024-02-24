Feature: DynamoDB based event store

    Scenario: Stream subscription
        Given a running dynamodb event store service
        And a subscription to the event store
        When an event is published to the event store
        Then the event is received by the subscription
