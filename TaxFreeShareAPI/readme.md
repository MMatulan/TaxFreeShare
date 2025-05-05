# TaxFreeShare Backend
# Github Repo:
https://github.com/MMatulan/TaxFreeShare.git

**Prosjektbeskrivelse** 
Dette er backend-delen av prosjektet TaxFreeShare, en applikasjon hvor kjøpere kan legge inn bestillinger
og selgere kan bekrefte og fullføre dem. Backend er bygget med .NET 8 Web API og bruker MySQL som database.

## Teknologi

 - ASP.NET Core Web API

 - Entity Framework Core

 - MySQL

 - JWT-autentisering

 - SignalR (sanntidskommunikasjon)

## Autentisering
 - JWT brukes for å autentisere brukere (både kjøper og selger).

 - Rollen (Role) og bruker-ID (NameIdentifier) er inkludert som claims i token.

 - Eksempel på brukerclaims:
 - {
   "sub": "1",
   "email": "user@example.com",
   "role": "Kjøper"
   }

## API-endepunkter
  Brukere
  Metode	    Endepunkt	            Beskrivelse
  POST	    /api/users/register	    Registrer ny bruker
  POST	    /api/users/login	    Logg inn og få JWT
  GET	    /api/users/me	        Hent egen brukerprofil


  Ordrer
  Metode	      Endepunkt	                Beskrivelse
  GET	        /api/orders	            Hent bestillinger for innlogget bruker
  GET	        /api/orders/{id}	    Hent én spesifikk ordre
  POST	        /api/orders	            Opprett ny ordre (krever kjøperrolle)
  PUT	        /api/orders/status/{id}	Oppdater ordrestatus (admin)
  POST	        /api/orders/assign/{orderId}	Selger bekrefter og tildeler seg en ordre
  DELETE	     /api/orders/{id}	Slett en ordre (admin)

  Produkter
  Metode	        Endepunkt	            Beskrivelse
  GET	        /api/products	        Hent alle produkter
  GET	        /api/products/{id}	    Hent detaljer for et spesifikt produkt
  POST	        /api/products	        Opprett et nytt produkt
  PUT	        /api/products/{id}	    Oppdater et eksisterende produkt
  DELETE	    /api/products/{id}	    Slett et produkt

  Transaksjoner
  Metode	        Endepunkt	            Beskrivelse
  POST	        /api/transactions	    Opprett en ny transaksjon for en ordre
  GET	        /api/transactions/{id}	Hent en spesifikk transaksjon (krever tilgang)
  PUT	        /api/transactions/{id}	Oppdater transaksjonsstatus (kun admin)

### Databasestruktur (MySQL)
  Tabeller
  - Users
    - Id, Email, PasswordHash, Role
  
  - Orders 
    - Id, UserId, SellerId, Status, OrderDate, TotalAmount
  
  - OrderItems 
    - Id, OrderId, ProductId, Quantity, Price
  
  - Products 
    - Id, Name, Category, Brand, Price


## SignalR (Chat)
  Hensikt: Sanntidschat mellom kjøper og selger
  
  Endepunkt: /chathub
  
  Metoder:
  
    JoinChat(groupName)
  
    SendMessageToGroup(groupName, senderName, message)

    ReceiveMessage (klient)

