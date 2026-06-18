## ADDED Requirements

### Requirement: Admin can create contact records
The system MUST allow an authenticated user with role `Admin` to create a new phonebook contact record.

#### Scenario: Admin creates contact successfully
- **WHEN** an authenticated `Admin` submits valid values for `Nombre` and `Telefono1`
- **THEN** the system SHALL create a `FichaContacto` with a unique `Guid`, assign `FechaAlta` automatically, and persist optional values for `Telefono2` and `Observaciones`

#### Scenario: Contact creation rejected by domain validation
- **WHEN** an authenticated `Admin` submits an invalid `Nombre` or invalid phone data
- **THEN** the system SHALL reject the request and SHALL not persist the `FichaContacto`

### Requirement: Authenticated users can consult phonebook contacts
The system MUST allow any authenticated user, regardless of role, to consult phonebook contacts.

#### Scenario: Authenticated user lists contacts
- **WHEN** an authenticated user requests the contact list
- **THEN** the system SHALL return the persisted `FichaContacto` records with `Id`, `FechaAlta`, `Nombre`, `Telefono1`, `Telefono2`, and `Observaciones`

#### Scenario: Authenticated user gets contact detail
- **WHEN** an authenticated user requests a contact by existing `Guid`
- **THEN** the system SHALL return the matching `FichaContacto` detail

### Requirement: Admin can update existing contact records
The system MUST allow an authenticated user with role `Admin` to edit an existing phonebook contact record.

#### Scenario: Admin updates contact successfully
- **WHEN** an authenticated `Admin` submits valid update data for an existing `FichaContacto`
- **THEN** the system SHALL persist updated values while preserving the original `Id` and `FechaAlta`

#### Scenario: Admin update fails for missing contact
- **WHEN** an authenticated `Admin` attempts to edit a non-existing `FichaContacto`
- **THEN** the system SHALL return a not found outcome and SHALL not create a new record implicitly

### Requirement: Admin can delete contact records
The system MUST allow an authenticated user with role `Admin` to delete an existing phonebook contact record.

#### Scenario: Admin deletes existing contact
- **WHEN** an authenticated `Admin` requests deletion of an existing `FichaContacto`
- **THEN** the system SHALL remove the record from persistence

#### Scenario: Admin delete fails for missing contact
- **WHEN** an authenticated `Admin` requests deletion of a non-existing `FichaContacto`
- **THEN** the system SHALL return a not found outcome