# secret-santa

## Libraries to note that were used to build this
- C#
  - .NET Core
  - NUnit
- JS
  - Lodash
  - React-bootstrap
  - React

## Prequisites to running locally
- Install Nodejs on your system
- Ensure you have .NET Core functionality installed on your Visual Studio instance

## Running locally
- If all dependenices are installed correctly you should be able to just run it like any other web app in Visual Studio
- The web app should open automatically and should allow you to hit the API from there.

## Secret Santa Api Reference
Hitting the API can be done using any tool such as curl, Postman, httpie, etc.

### Name list API spec:
```
Route: '{{localInstanceUrl}}/api/secretsanta/pair'
Body: [ (family array)
  {
    name: string
    members: object {
      name: string
    }
  }
]

Error cases:
  1. Too large of a family: A single family's members cannot be more than half of the total members 
  input across all families.
  2. Family with one member: each family must have at least 2 members
  3. Duplicate family names: Each family must have a unique name (could fix this soon)
  4. Duplicate family member names: Each member across all families must have a unique name (also could be fixed)
  5. Insufficient families: At least 2 families must be put in the system.

```
