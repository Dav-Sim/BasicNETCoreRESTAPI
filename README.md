# Basic .NET Core REST API
**Basic example of API in NET Core 3.1**   
   
Strictly speaking, this API is not fully RESTful, but it's relatively close to that. Everything is written very simply so that it can be understood, as I hope.  

### REST constraints
First little theory, and what we have in our example.  

1) Client–server architecture - **YES**
2) Statelessness - **YES**
3) Cacheability - ***partially***
4) Layered system - **YES**
5) Code on demand (optional) - ***Not planned*** in our case
6) The uniform interface constraint
    - Resource identification in requests - **YES**
    - Resource manipulation through representations - **YES** (in case of datashaping helps us HATEOAS to adhere this constraint)
    - Self-descriptive messages - **YES**
    - Hypermedia as the engine of application state (HATEOAS) - **YES** (***partially***)

## Recently added
- Added support for Caching headers (ETag, Cache-Control...) and support for request with headers If-None-Match (GET/HEAD) and If-Match (PUT/PATCH)
- Added vendor specific media type which contains HATEOAS links
- Added support for UPSERT (create resource using PUT or PATCH)
- Added Pagination
- Added Sorting
- Added collection controller, for GET collection of tasks by ids, or POST collection of tasks - create multiple task at one request.

## What is inside

### Content negotiation
---
Works correctly with media types like "application/json" or "application/xml" otherwise it returns 406 (not acceptable) also supports custom media type.

### Http Status codes
---
Returns correct status codes
- 2xx for correct operation
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

### Filtering, Sorting, Pagination
---
These components must work together. 
#### Filtering
Filtering, sorting etc. in query string can be long and messy if we add all possible parameters to method arguments. In example is used Custom object to do this. 
instead of many [FromQuery] arguments we use  
`([FromQuery] TaskResourceParameters parameters)`

#### Sorting
Sorting can be a little more complicated if the client receives a DTO and not directly ENTITY. In this example is created a mapping between ENTITY properties and DTO properties. 

#### Pagination
Pagination is necessary for larger data collections. You also need to have some default values if the client does not specify pagination.
Correctly, the GET request should also return a link to the previous and next page in HTTP Headers (this link contains also data about other filters, sorting etc.) This is applied in this example. 

### Upserting
---
Upserting can be a useful ability in some cases. This means that you can use PATCH or PUT to create a resource. It is especially suitable for child elements.

### HATEOAS Links
---
if request accepts media type "application/json" (or not contains Accept at all) API returns representation of object/s as json. Nothing more is added to response body.
But if request accepts 
- "application/vnd.todoapi.task.hateoas+json" (for get one task)
- "application/vnd.todoapi.tasks.hateoas+json" (for collection)
than response contains "value" and "links" properties.
Single task request ('/api/tasks/{id}')  

        {
            "id": "01",
            "name": "Task1",
            "description": "Desc for Task1",
            "priority": 1,
            "status": "NotStarted",
            "links": [
                {
                    "href": "http://localhost:59786/api/tasks/01",
                    "rel": "self",
                    "method": "GET"
                },
                {
                    "href": "http://localhost:59786/api/tasks/01",
                    "rel": "delete",
                    "method": "DELETE"
                }
            ]
        }
in case of collection ('/api/tasks') it contains also links for next page and previous page.

### Cache headers
---
In example is created simple custom middleware which adds ETag, Cache-Control etc. to response headers. 
Middleware should be setup globally in startup.cs (ConfigureServices), or partially by class or methods attributes.  

Middleware in case of GET/HEAD request with **If-None-Match** header, will serve 304-NotModified (if not modified).  

In case of PUT/PATCH request with **If-Match** header, will serve 412 Precondition failed (if ETag not match)  

ETag is MD5 hash of response

Example of headers

    Cache-Control=public,max-age=120,must-revalidate
    Expires=Wed, 17 Mar 2021 20:21:26 GMT
    Last-Modified=Wed, 17 Mar 2021 20:19:26 GMT
    ETag="287C17D93C066AE2B4065361415D2EFD"


### Postman test
---
In repo you can find postman collection file, in this collection are few tests of basic API functionality.
![Postman image][postman]

[postman]: https://github.com/Dave4626/BasicNETCoreRESTAPI/blob/main/postman.gif "Postman image"

