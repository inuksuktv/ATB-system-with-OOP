# ATB-system-with-OOP

This project was completed as part of Unity's Junior Programmer pathway. The task was to create a game while demonstrating the four pillars of Object-Oriented Programming: Abstraction, Inheritance, Polymorphism, and Encapsulation. Here I have created an ATB-style turn-based battle system.

ABSTRACTION:
The most notable application of this principle is within the Unit State Machine script. The actions taken within each state are abstracted to their own methods. This is supremely useful as it allows us to completely redefine the actions for a unit that inherits from the UnitStateMachine class, such as HeroStateMachine.

INHERITANCE:
The most notable examples of inheritance are the classes that define our units, with BaseUnit defining several stats for the units, UnitStateMachine inheriting from BaseUnit and defining basic unit behaviour in combat, and the HeroStateMachine that inherits from UnitStateMachine.

POLYMORPHISM:
UnitStateMachine contains two virtual methods that are overriden in the HeroStateMachine, since the heroes behave differently in combat than basis enemies. Method overriding was not used in this project.

ENCAPSULATION:
With the AttackHandler script which is responsible for holding attack information and passing it to the BattleStateMachine, the attackerName and attackDescription check that the string length is not longer than 10 and 20 characters respectively. Each string has a private field within the class, and the setter is validated before it's passed to the backing field.
