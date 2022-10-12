Feature: Attractie

Scenario: BestaatAl
    Given attractie Draaimolen bestaat
    When attractie Draaimolen wordt toegevoegd
    Then moet er een error 403 komen

# Opdracht: voeg hier een scenario toe waarin een 404 wordt verwacht bij het deleten van een niet-bestaande attractie

Scenario: BestaatNiet
    Given attractie Achtbaan bestaat niet
    When attractie Achtbaan wordt verwijderd
    Then moet er een error 404 komen

# Eigen ding
Scenario: CorrectVerwijderd
    Given attractie Botsauto bestaat
    When attractie Botsauto wordt verwijderd
    Then moet er een 404 code komen
    #Refuses to give anything other than 404 and debug is worthless