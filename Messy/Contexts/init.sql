CREATE TABLE Users
(
    Id        BIGSERIAL PRIMARY KEY,
    Name      VARCHAR(255) NOT NULL,
    UserName  VARCHAR(255) NOT NULL,
    Password  VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP    NOT NULL,
    UpdatedAt TIMESTAMP,
    DeletedAt TIMESTAMP
);

CREATE TABLE UserProfiles
(
    Id          BIGSERIAL PRIMARY KEY,
    Description VARCHAR(500),
    UserId      BIGINT    NOT NULL,
    CreatedAt   TIMESTAMP NOT NULL,
    UpdatedAt   TIMESTAMP,
    DeletedAt   TIMESTAMP
);

ALTER TABLE UserProfiles
    ADD CONSTRAINT FK_UserProfile_User FOREIGN KEY (UserId) REFERENCES Users (Id) on delete cascade ;

CREATE TABLE Chats
(
    Id        BIGSERIAL PRIMARY KEY,
    Title     VARCHAR(50),
    Type      INT       NOT NULL,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP,
    DeletedAt TIMESTAMP
);

CREATE TABLE ChatUsers
(
    Id        BIGSERIAL PRIMARY KEY,
    UserId    BIGINT    NOT NULL,
    ChatId    BIGINT    NOT NULL,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP,
    DeletedAt TIMESTAMP
);

ALTER TABLE ChatUsers
    ADD CONSTRAINT FK_ChatUser_User FOREIGN KEY (UserId) REFERENCES Users (Id);

ALTER TABLE ChatUsers
    ADD CONSTRAINT FK_ChatUser_Chat FOREIGN KEY (ChatId) REFERENCES Chats (Id);

CREATE TABLE Messages
(
    Id         BIGSERIAL PRIMARY KEY,
    Body       VARCHAR(5000) NOT NULL,
    ParentId   BIGINT,
    ChatUserId BIGINT        NOT NULL,
    CreatedAt  TIMESTAMP     NOT NULL,
    UpdatedAt  TIMESTAMP,
    DeletedAt  TIMESTAMP
);

ALTER TABLE Messages
    ADD CONSTRAINT FK_Message_Parent FOREIGN KEY (ParentId) REFERENCES Messages (Id);

ALTER TABLE Messages
    ADD CONSTRAINT FK_Message_ChatUser FOREIGN KEY (ChatUserId) REFERENCES ChatUsers (Id);

CREATE TABLE Roles
(
    Id       BIGSERIAL PRIMARY KEY,
    Priority SMALLINT    NOT NULL,
    Name     VARCHAR(70) NOT NULL,
    Slug     VARCHAR(50) NOT NULL
);

CREATE TABLE Permissions
(
    Id        BIGSERIAL PRIMARY KEY,
    Name      VARCHAR(50) NOT NULL,
    Slug      VARCHAR(50),
    CreatedAt TIMESTAMP   NOT NULL,
    UpdatedAt TIMESTAMP,
    DeletedAt TIMESTAMP
);

CREATE TABLE RolePermissions
(
    RoleId       BIGINT NOT NULL,
    PermissionId BIGINT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId)
);

ALTER TABLE RolePermissions
    ADD CONSTRAINT FK_RolePermission_Role FOREIGN KEY (RoleId) REFERENCES Roles (Id);

ALTER TABLE RolePermissions
    ADD CONSTRAINT FK_RolePermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions (Id);

CREATE TABLE ChatUserPermissions
(
    ChatUserId   BIGINT NOT NULL,
    PermissionId BIGINT NOT NULL,
    PRIMARY KEY (ChatUserId, PermissionId)
);

ALTER TABLE ChatUserPermissions
    ADD CONSTRAINT FK_ChatUserPermission_ChatUser FOREIGN KEY (ChatUserId) REFERENCES ChatUsers (Id);

ALTER TABLE ChatUserPermissions
    ADD CONSTRAINT FK_ChatUserPermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions (Id);

CREATE TABLE ChatUserRoles
(
    ChatUserId BIGINT NOT NULL,
    RoleId     BIGINT NOT NULL,
    PRIMARY KEY (ChatUserId, RoleId)
);

ALTER TABLE ChatUserRoles
    ADD CONSTRAINT FK_ChatUserRole_ChatUser FOREIGN KEY (ChatUserId) REFERENCES ChatUsers (Id);

ALTER TABLE ChatUserRoles
    ADD CONSTRAINT FK_ChatUserRole_Role FOREIGN KEY (RoleId) REFERENCES Roles (Id);

CREATE TABLE MessageFiles
(
    MessageId BIGINT NOT NULL,
    FileId    BIGINT NOT NULL,
    PRIMARY KEY (MessageId, FileId)
);



CREATE TABLE Files
(
    Id          BIGSERIAL PRIMARY KEY,
    FileName    VARCHAR(255) NOT NULL,
    FilePath    TEXT         NOT NULL,
    FileSize    BIGINT       NOT NULL,
    ContentType VARCHAR(100),
    CreatedAt   TIMESTAMP    NOT NULL,
    UpdatedAt   TIMESTAMP,
    DeletedAt   TIMESTAMP
);

ALTER TABLE MessageFiles
    ADD CONSTRAINT FK_MessageFile_Message FOREIGN KEY (MessageId) REFERENCES Messages (Id);

ALTER TABLE MessageFiles
    ADD CONSTRAINT FK_MessageFile_File FOREIGN KEY (FileId) REFERENCES Files (Id);
