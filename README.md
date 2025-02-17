# FunBooksAndVideos

To run this project, either install SqlExpress, or change the connection string at `DataAccess.DatabaseContext._localSqlServerName`. Once this is done, start the Api project - this should create the database automatically.

I have assumed the that we are required to accept item lines in the format defined in the excercise. Initially, the database is seed with a customer with ID 1, and the following four products:
- Video "Comprehensive First Aid Training"
- Book "The Girl on the train"
- Video Club Membership
- Book Club Membership

The below json should result in a PurchaseOrder being successfully processed:
`{
  "purchaseOrderId": 2,
  "customerId": 1,
  "total": 43.32,
  "itemLines": [
    "Book \"The Girl on the train\"",
    "Book Club Membership"
  ]
}`

