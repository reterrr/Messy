INSERT INTO Permissions (Name, Type)
VALUES ('AddToChat', 0),
       ('RemoveFromChat', 1),
       ('EditChat', 2),
       ('MessageToChat', 3),
       ('BanInChat', 4);


insert into rolepermissions(roleid, permissionid)
values (1, 1),
       (1, 2),
       (1, 3),
       (1, 4),
       (1,5),
       (2, 1),
       (2, 2),
       (2, 4),
       (3,1),
       (3,4)