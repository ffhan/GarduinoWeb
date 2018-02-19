# Garduino Project

Ovo je repozitorij za ASP.NET aplikaciju za projekt. Objašnjenje samog projekta se nalazi na [glavnom repozitoriju sistema](https://github.com/ffhan/garduino) i na [stranici](http://garduinoproject.azurewebsites.net/)

Ukratko - napravio sam web stranicu i pozadinski web API koji komununicira s Arduino uređajem koji mjeri podatke u real-timeu i vraća ih serveru, te prima naredbe od servera. To znači da se može kontrolirati uređaj i pratiti rad uređaja bez da osoba mora biti fizički prisutna.

Testni račun: 
email: admin@admin.org
password: Admin_projekt0

Na help stranici je pojašnjeno što se zbiva, na garduino repozitoriju je sve objašnjeno u još veće detalje.

Uređaj bi trebao svakih pola sata unijeti nove vrijednosti u bazu podataka te javlja serveru da je živ i njegovo unutarnje stanje, što znači da možete slati naredbe koje će izvoditi i pritom dobiti natrag informacije o promjenama.

## Garduino web API
* RESTful web api
* Enables any device to connect to the server, log in, register, send its' data (currently strictly limited), issue and receive 
commands from other devices
* Enables monitoring and communication between devices not directly connected

## Garduino web app
* Enables monitoring and control of devices through browsers
* Gives the user exceptional insight into data and innerworkings of their systems.

## [Garduino main](https://github.com/ffhan/garduino) - Main control system for my particular system
* System designed to control all aspects of indoor plant growing, specifically chili plants.
