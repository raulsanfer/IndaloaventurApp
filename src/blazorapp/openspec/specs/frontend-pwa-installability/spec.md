# frontend-pwa-installability Specification

## Purpose
TBD - created by archiving change habilitar-instalacion-pwa-webapp. Update Purpose after archive.
## Requirements
### Requirement: PWA install metadata
The frontend MUST publish valid PWA metadata so that compatible browsers can evaluate the app as installable.

#### Scenario: Manifest is exposed from the app shell
- **WHEN** a user opens the application shell in the browser
- **THEN** the HTML document includes a reference to a web app manifest served by the web project

#### Scenario: Manifest declares installable app properties
- **WHEN** the browser fetches the manifest
- **THEN** it receives application metadata including `name`, `short_name`, `start_url`, `display`, `theme_color`, `background_color`, and install icon entries sized for PWA usage

### Requirement: PWA install icons
The frontend MUST provide application icons suitable for installation prompts and home screen shortcuts, using the official project logo as the visual base.

#### Scenario: Required icon sizes are available
- **WHEN** the manifest references application icons
- **THEN** the referenced files exist in the web app and include at least install-oriented sizes equivalent to `192x192` and `512x512`

#### Scenario: Branding stays aligned with the app
- **WHEN** the user sees the installed app icon from the browser or home screen
- **THEN** the icon is derived from the official IndaloAventurApp logo rather than a generic browser favicon

### Requirement: Service worker registration
The frontend MUST register a service worker in secure contexts so the browser can treat the application as a PWA candidate.

#### Scenario: Service worker is registered on supported browsers
- **WHEN** the app is opened from a secure origin in a browser with service worker support
- **THEN** the frontend attempts to register a service worker from the web project

#### Scenario: Unsupported environments do not break the app
- **WHEN** the app runs in a browser or context without service worker support
- **THEN** the application continues to load without throwing blocking runtime errors

### Requirement: Installability validation
The project MUST be verifiable as installable in supported browsers through a documented validation flow.

#### Scenario: Local HTTPS build can be evaluated for installation
- **WHEN** the app is launched with the HTTPS profile in development
- **THEN** the resulting site can be checked in browser installability tooling without missing mandatory PWA artifacts

#### Scenario: Standalone launch is validated manually
- **WHEN** the app is installed from a compatible browser
- **THEN** the validation checklist confirms that it opens as a standalone app entry point rather than only as a browser tab

