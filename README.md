# Basic .NET Core REST API
**Basic example of API in NET Core 3.1**   
   
This API is not fully "RESTful" - it lacks lot of "advanced" things, like HATEOAS, Caching, Concurrency, data-shaping etc. but anyway it is a good starting point...

## News
- Added collection controller (can be added or retrieved multiple tasks at one request)
- Added child class "task detail" and controller for it
- Updated postman tests

## What is inside

### Content negotiation
---
Works correctly with media types like "application/json" or "application/xml" otherwise it returns 406 (not acceptable)

### Http Status codes
---
Returns correct status codes
- 200/201/204 for correct operation
- 4xx for consumer errors
- eventually 5xx for server errors

### Http methods
---
- OPTIONS 
- GET
- HEAD
- POST
- PUT
- PATCH
- DELETE  

### Supports patch with JsonPatchDocument
---
This API supports json patch standard **RFC 6902**.  
Using Microsoft.AspNetCore.JsonPatch.JsonPatchDocument and Microsoft.AspNetCore.Mvc.NewtonsoftJson  
Example of patch document  

    [
        {
            "op": "replace",
            "path": "/name",
            "value": "Patched name"
        },
        {
            "op": "remove",
            "path": "/status"
        }
    ]

### Entity and data transfer object separation
---
In example is separated entity model, from data transfer objects (DTO).  

To reduce the amount of code, but at the same time maintaining the possibility to separate validation of object for creation and for update, we have in project base DTO class which has common validation rules, and two derived DTO classes. One for creating and one for update.

To convert between entities and DTOs is used **AutoMapper**  
AutoMapper.Extensions.Microsoft.DependencyInjection

### Validation
---
For validation is mainly used System.ComponentModel.DataAnnotations attributes, and for class validation there is one custom attribute (which validates whole class).  
Other option could be implementing IvalidatableObject, but in this demo attributes seemd more versatile to me.

### Filtering GET result
---
Filtering, sorting etc. in query string can be long and messy if we add all possible parameters to method arguments. In example is used Custom object to do this. 
instead of many [FromQuery] arguments we use  
`([FromQuery] TaskResourceParameters parameters)`

### Postman test
In repo you can find postman collection file, in this collection are few tests of basic API functionality.
![Postman image][postman]

[postman]: https://github.com/Dave4626/BasicNETCoreRESTAPI/blob/main/postman.gif "Postman image"

