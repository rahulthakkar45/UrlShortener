## 1. Redirect Status Code: 

The redirect status code is 302 which indicates that the requested resource has been temporarily moved to a different URI. This status code is commonly used for temporary redirects, and it informs the client that it should continue to use the original URI for future requests.

So we should consider using the 302 status code when we want to temporarily redirect users to a different page or resource, while still allowing them to access the original URI in the future.

A 302 redirect is appropriate in scenarios where the change is not permanent, such as during maintenance or when testing new features. It is important to note that search engines may not update their indexes for 302 redirects, as they are considered temporary.

Also we can able to implement the 301 redirect status code in future if we want to make the redirection permanent. This would inform clients and search engines that the resource has been permanently moved to a new location, and they should update their records accordingly.

---

## 2. URL Deletion/ disable strategy:

When it comes to deleting URLs, there are two main approaches: physical delete and soft delete.

In this application, we have decided to implement a soft delete approach. This means that when a user deletes a resource, it will not be permanently removed from the database.

When URL is deleted, 'IsActive' field will be set to 'false'.

Also we can not used it for redirection purpose because the URL will still exist in the database, but it will be marked as inactive. This allows us to maintain a record of deleted URLs and potentially restore them in the future if needed.

We use this approach because it allows you to take advantage of the benefits of soft deletion, such as the ability to recover deleted data and maintain historical records. It also helps prevent accidental data loss and provides a way to track changes over time.

If URL is deleted once so we can not use that url for the redirection purpose but in future suppose we use this URL again then we can set 'IsActive' field to 'true' so it also prserves the our historical data of that url as like ClickCount and etc as well.

Also we can implement the physical delete approach in future if we want to permanently remove the resource from the database. This would free up storage space and ensure that the data is no longer accessible.

---

## 3.Click Count Strategy:

Click count is an important metric that helps us understand how many times a particular URL has been accessed or clicked by users. In this application, we have decided to implement a click count strategy to track the number of clicks for each URL.
When a user clicks on a URL, the click count will be incremented by 1. This allows us to keep track of the popularity and usage of each URL over time.

We have implemented a click count startegy using the Aggregation from the Click Table in the database. This means that we will be able to retrieve the total click count for each URL by aggregating the click data from the Click Table.

This approach allow us to keeps click count consistent and accurate, as it ensures that all clicks are recorded and counted correctly. It also allows us to easily retrieve the click count for each URL, which can be useful for analytics and reporting purposes.

This Click Count Strategy is important for understanding user behavior and engagement with the application. It can help us identify popular URLs, track trends over time, and make data-driven decisions to improve the user experience.

Also we can implement a more advanced click count strategy in future if we want to track additional metrics, such as unique clicks, click-through rates, and conversion rates. This would provide us with a more comprehensive understanding of user engagement and help us optimize our application for better performance.

---

## 4. Concurrency Handling:

Short codes are protected by a database unique Index, which ensures that each short code is unique and prevents duplicate entries. This unique index acts as a constraint on the database, preventing any attempts to insert a short code that already exists.

For that we have implemented a unique Index on the 'shortCode' using EF Core:

       builder.Entity<Url>().HasIndex(u => u.ShortCode).IsUnique();

Also on the API side we have checked the existence of the short code before inserting a new URL. If the short code already exists, we return an error response to the client, indicating that the short code is already in use.

---

## 5. Statistics and Time Zone:

We are calculating the URL statistics from the Click Table in the database. This allows us to retrieve information about the number of clicks, the time of each click, and other relevant data.

We have stored the timezone in UTC format in the database. This allows us to standardize the time representation and avoid any confusion related to different time zones.
When we retrieve the statistics.

For that we have used like DateTime.UtcNow for the field like createdAt and c.ClickedAt in the Click Table. This ensures that all timestamps are stored in a consistent format, regardless of the user's local time zone.
When we retrieve the statistics.

Also we can store the the timezone information in UTC but based on the user's preference we can convert it to their local timezone when displaying the statistics. This would provide a more user-friendly experience and allow users to view the data in a format that is relevant to them.