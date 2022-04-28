# ATB-system-with-OOP

This project was completed as part of Unity's Junior Programmer pathway. The task was to create a game while demonstrating the four pillars of Object-Oriented Programming: Abstraction, Inheritance, Polymorphism, and Encapsulation. Here I have created an ATB-style turn-based battle system.

ABSTRACTION:

The most notable application of this principle is within the State Machine scripts for the actors in battle. The desired actions from the actor are typically abstracted into a method that gets called from within that state. Observe the Update() method within any State Machine script.

INHERITANCE:

The most notable examples of inheritance are the Hero State Machines and Enemy State Machines that inherit from BaseUnit. BaseUnit holds data like HP, ATK, and the attackList whereas the State Machines are responsible that actor's part in the battle logic.
Another example of inheritance are the attacks in each Hero and Enemy's attackList. These attacks inherit from BaseAttack and are passed to the actionQueue along with the attackTarget and attackerGameObject.

POLYMORPHISM:
Maybe we'll override the DoDamage function or something similar?

ENCAPSULATION:
We don't have to get too fancy here. Keep DoDamage dealing positive amounts of damage? Keep ability descriptions to a particular length?
