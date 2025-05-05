# TaxFreeShare Frontend
# Github Repo:
https://github.com/MMatulan/TaxFreeShare.git

## Prosjektbeskrivelse
Dette er frontend-delen utviklet med Blazor WebAssembly. 
Den kommuniserer med backend via HTTP og bruker SignalR for chat.

## Teknologi

 - Blazor WebAssembly

 - Bootstrap

- HttpClient

 - SignalR

 - JWT-håndtering via LocalStorage

## Autentisering
 - JWT-token lagres i localStorage

 - Token brukes ved kall til API

 - Autentisering og rollebasert visning i UI

## Funksjoner
 - Kjøper
   - Legg inn bestilling
   - Se egne bestillinger

 - Selger
   - Se åpne bestillinger
   - Bekrefte bestillinger (de flyttes til "mine bestillinger")

 - Chat
   - Sanntidsmeldinger mellom kjøper og selger

 - Profil

## Komponentstruktur

    - Chat.razor
    - DashboardKjøper.razor
    - DashboardKjøper2.razor
    - ForgotPassword.razor
    - Home.razor
    - Login.razor
    - MineBestillingerKjøper.razor
    - MineBestillingerSelger.razor
    - NyBestilling.razor
    - Nye-forespørsler.razor
    - OppdaterBestilling.razor
    - RedigerProfil.razor
    - Register.razor
    - ResetPassword.razor

## Tjenester
 - OrderService.cs 
    – håndterer HTTP-kall for ordrer
 - UserService.cs
   - håndterer HTTP-kall for brukere
 - AuthService.cs
   - håndterer autentisering og JWT-håndtering
   - håndterer lagring og uthenting av token fra localStorage
 - IAuthService.cs
   - grensesnitt for AuthService
 - ProductService.cs
   - håndterer HTTP-kall for produkter

## Lokal kjøring
1. Start TaxFreeShareAPI (backend)

2. Start TaxFreeShareFrontend3 (frontend)

3. Åpne i nettleser: http://localhost:5200

