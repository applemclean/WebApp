# Users management using ASP.NET and WCF (windows service, IIS hosted service)

## Installation
- Open ports:
    - For the purose of running tests: $ netsh http add urlacl url=http://+:8090/ user=YOUR_USERNAME
    - For the purpose of running as windows service: $ netsh http add urlacl url=http://+:80/UsersApi user=YOUR_USERNAME
- Clone the repository
- Build solution
- (Optional) restore the databases from /dump folder
- Start WebApp project (Users service as IIS hosted service)

## Users service as windows service
- Install the project
- Open WebApp/Scripts/App/Services/UsersStore.js
- Uncomment // var usersApi = 'http://localhost/UsersApi/users/:id'; // (hosted as service)
- Comment out var usersApi = 'Api.svc/users/:id'; // (hosted in IIS)
- Install the service - $ installutil "PATH-TO-SERVICE/DatabaseService.WindowsService.exe"
- Start the service $ net start windatabaseservice
- Run WebApp project

## Running the tests
- Use visual studio test runner - Run All option

## License (MIT)
Copyright (c) 2016 Vladimir Kiselev

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
