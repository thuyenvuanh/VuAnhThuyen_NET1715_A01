USE HotelManagement;
GO

-- Inserting data into the Customer table
INSERT INTO CUSTOMER (Id, FullName, Telephone, Email, Birthday, [Status], [Password])
VALUES (1,'John Doe', '1234567890', 'johndoe@example.com', '1980-01-01', 0, 'password1'),
       (2, 'Jane Smith', '0987654321', 'janesmith@example.com', '1985-02-02', 0, 'password2');
GO

-- Inserting data into the ROOM_TYPE table
INSERT INTO ROOM_TYPE (Id, [Name], [Description], Note)
VALUES (1, 'Single Room', 'A room with a single bed', 'Ideal for solo travelers'),
       (2, 'Double Room', 'A room with a double bed', 'Ideal for couples');
GO

-- Inserting data into the ROOM table
INSERT INTO ROOM (Id, Number, [Description], MaxCapacity, [Status], PricePerDate, TypeId)
VALUES (1, 101, 'First floor, near the elevator', 1, 0, 100.00, 1),
       (2, 102, 'First floor, with a view of the garden', 2, 0, 200.00, 2);
GO

-- Inserting data into the RESERVATION table
INSERT INTO RESERVATION (Id, OnDate, RoomId, CustomerId, [Status])
VALUES (1, '2024-06-01', 1, 1, 'CONFIRMED'),
       (2, '2024-06-02', 2, 2, 'PENDING'),
       (3, '2024-06-03', 1, 2, 'DONE'),
       (4, '2024-06-04', 2, 1, 'CANCELLED');
GO