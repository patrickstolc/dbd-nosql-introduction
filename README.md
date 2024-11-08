# MongoDB Blogging Platform

A blogging platform implementation using MongoDB and .NET Core, demonstrating document database design patterns and REST API development.

## Data Model Design

### Schema Overview

```json
User {
"id": ObjectId,
"name": string,
"handle": string,
"createdAt": DateTime,
"updatedAt": DateTime
}
Blog {
"id": ObjectId,
"title": string,
"description": string,
"userId": string,
"createdAt": DateTime,
"updatedAt": DateTime
}
Post {
"id": ObjectId,
"title": string,
"content": string,
"userId": string,
"userName": string,
"blogId": ObjectId,
"comments": [
{
"id": ObjectId,
"content": string,
"userId": string,
"userName": string,
"createdAt": DateTime,
"updatedAt": DateTime
}
],
"createdAt": DateTime,
"updatedAt": DateTime
}
```

### Design Decisions and Trade-offs

#### 1. Embedded Comments in Posts

**Benefits:**

- Single query retrieves post with all its comments
- Atomic updates for comment operations
- Better read performance
- Maintains data consistency within a single document
- Simplified transaction handling

**Trade-offs:**

- Document size might grow large with many comments
- Cannot query comments independently
- Updates affect the entire document

#### 2. Denormalized UserName

**Benefits:**

- Reduces join operations for display purposes
- Faster read performance
- Better user experience with immediate data availability

**Trade-offs:**

- Data duplication
- Requires cascading updates when username changes
- Additional storage space
- Potential for temporary inconsistency

#### 3. Separate Blog Collection

**Benefits:**

- Clean separation of blog metadata from posts
- Efficient blog listing and searching
- Easier to manage blog-level permissions
- Scalable for blog-specific features

**Trade-offs:**

- Requires joins for complete blog data
- Additional collection to manage

#### 4. Timestamp Fields

**Benefits:**

- Built-in audit trail
- Enables sorting by creation/update time
- Supports caching strategies
- Helps with synchronization

**Trade-offs:**

- Additional storage space
- Must be maintained on every update

## Implementation Details

### API Endpoints

1. **Blog Posts Retrieval**

   ```
   GET /api/post/blog/{blogId}/posts
   ```

   - Retrieves all posts for a specific blog
   - Includes embedded comments for efficiency

2. **Post Comments**

   ```
   GET /api/post/{postId}/comments
   ```

   - Retrieves comments for a specific post
   - Leverages embedded document structure

3. **Post Content Update**

   ```
   PUT /api/post/{postId}/content
   ```

   - Updates post content
   - Atomic operation

4. **Username Update**
   ```
   PUT /api/user/{userId}/username
   ```
   - Updates username across all references
   - Handles denormalized data updates

### Performance Considerations

1. **Read Performance**

   - Embedded comments optimize read operations
   - Denormalized username reduces joins
   - Single-query access patterns

2. **Write Performance**

   - Atomic updates within single documents
   - Cascading updates handled efficiently
   - Balanced write consistency vs. complexity

3. **Scalability**
   - Document model supports horizontal scaling
   - Independent collections allow for sharding
   - Efficient query patterns for common operations

### Security and Data Integrity

1. **Data Validation**

   - ObjectId validation in controllers
   - Strong typing in C# models
   - Exception handling for data operations

2. **Error Handling**
   - Consistent error responses
   - Proper HTTP status codes
   - Detailed error messages in development
