# TaxFreeShare

**TaxFreeShare** er en backend-drevet applikasjon hvor reisende kan tjene penger ved å kjøpe taxfree-varer for andre personer. 
Plattformen kobler sammen kjøpere og selgere (reisende) og legger til rette for en trygg og enkel bestillingsflyt.

## Funksjoner

- Registrering og innlogging for både kjøpere og selgere
- Rollebasert autentisering med JWT
- Kjøper kan:
    - Opprette bestilling med ønskede produkter
    - Se og oppdatere sine bestillinger
- Selger kan:
    - Se tilgjengelige bestillinger
    - Bekrefte og tildele seg bestillinger
- Automatisk e-postbekreftelse ved bestilling
- RESTful API

## Teknologi brukt
- C# og .NET
- Entity Framework Core
- MYSQL Server
- Docker
- JWT for autentisering
- Blazor for frontend
- Bootstrap for UI
- SMTP3 for e-postsending

## Installasjon og kjøring

### Backend (API)

1. Klon repoet:
   ```bash
   git clone https://github.com/brukernavn/TaxFreeShare.git
   cd TaxFreeShare/TaxFreeShareAPI

## Autentisering
JWT-token blir generert ved innlogging og må inkluderes i Authorization-header (Bearer token) ved API-kall.

Roller: Kjøper, Selger

## E-postfunksjon
Ved opprettelse av bestilling blir det sendt e-post til kjøper med ordrebekreftelse. SMTP må konfigureres i appsettings.json.

- Videre utvikling
Betalingsløsning (Stripe/Vipps)

Meldingssystem mellom kjøper og selger

Bedre UI med mer brukervennlig dashboard

Admin-panel

Enhetstester og integrasjonstester

- Utvikler
  Matulan Mahenthra
  Backend-utvikling, API-design og fullstack-integrasjon
  📧 matulan.mahenthra@gmail.com