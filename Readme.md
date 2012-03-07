## Request Header Authorize Attribute for an ASP.NET MVC application
This library gives the option for an ASP.NET MVC application to accept authorizing credentials via a custom request Http Header. A good example of this is an OAuth Token, Basic Authentication or some custom Api key.

So how does the Token actually verify that it's legit? Well .. that's the only thing you need to implement. I have no idea how to talk to your database or whatever, so you need to define that.

###Required Arguments
```Authorization : a class that impliments ```ICustomAuthorization```. (I explain why, below).

###Optional Arguments
```RequireSsl:``` require the request to be secure? Default is ```false```, and ```LocalHost``` is ignored.
```Header:``` what header key should we check for? Default is ```Authorization```.

###Notes
The library includes an ``InMemory`` CustomAuthorization class .. which is just an in memory Dictionary .. pretending it's a database. This can be used for testing or R&D purposes.

So lets see how we can do this :)

## Code speaks.

```c#
[RequestHeaderAuthorize(Authorization = new InMemoryCustomAuthorization("aaa", "Pew Pew"))]
public ActionResult Post(PostInputModel postInputModel)
{ ... }
```

Here, i'm saying that this ActionMethod requires some Token to be provided in the header. Header key is defaulted to ```Authorization```.
Secondly, I've hardcoded the list of Tokens <-> User Names. (Remember, I said this is a contrite example).
So the following header will Authorize the request: ```Authorization: aaa```

Lets try another example, this time with a real ```ICustomAuthorization.cs``` class ...

```c#
public class MyDatabaseRequestHeaderAuthorization : ICustomAuthorization.cs
{
    private readonly IDatabase _myDatabase;

    public MyDatabaseRequestHeaderAuthorization (IDatabase myDatabase)
    {
        _myDatabase = myDatabase;
    }

    #region ICustomAuthorization Members

    public string AuthenticationType
    {
        get { return "MyDatabaseRequestHeaderAuthorization "; }
    }

    public bool TryAuthorize(string token, out IPrincipal principal)
    {
        principal = null;

        // *** THIS IS YOUR CUSTOM DATABASE STUFF.
        //     CHANGE THIS WITH .. ER .. WHATEVER YOU'RE USING.
        var user = myDatabase.FindUserByToken(token);

        if (user == null)
        {
            return false;
        }

        var identity = new GenericIdentity(_tokensAndNames[token], AuthenticationType);
        principal = new GenericPrincipal(identity, null);

        return true;
    }

    #endregion
}

[TokenAuthorize(Authorization = new MyDatabaseRequestHeaderAuthorization(myDatabase), RequireSsl=true, Header="XXX-Token")]
public ActionResult Post(PostInputModel postInputModel)
{ ... }
```

Ok .. so here we are saying that 
1. We need to have a secure request
2. The header key we'll look for is ```XXX-Token```
3. We ask the database to return the user with the provided token, assuming one was provided correctly.


### Pull Requests? 
Awww yeah! I do accept em *hint thint*
