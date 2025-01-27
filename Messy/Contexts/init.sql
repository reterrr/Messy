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
    ADD CONSTRAINT FK_UserProfile_User FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE;

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
    ADD CONSTRAINT FK_ChatUser_User FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE;

ALTER TABLE ChatUsers
    ADD CONSTRAINT FK_ChatUser_Chat FOREIGN KEY (ChatId) REFERENCES Chats (Id) ON DELETE CASCADE;

CREATE TABLE Messages
(
    Id         BIGSERIAL PRIMARY KEY,
    Body       VARCHAR(5000) NOT NULL,
    ParentId   BIGINT        NULL,
    ChatUserId BIGINT        NOT NULL,
    CreatedAt  TIMESTAMP     NOT NULL,
    UpdatedAt  TIMESTAMP,
    DeletedAt  TIMESTAMP
);

ALTER TABLE Messages
    ADD CONSTRAINT FK_Message_Parent FOREIGN KEY (ParentId) REFERENCES Messages (Id) ON DELETE SET NULL;

ALTER TABLE Messages
    ADD CONSTRAINT FK_Message_ChatUser FOREIGN KEY (ChatUserId) REFERENCES ChatUsers (Id) ON DELETE CASCADE;

CREATE TABLE Roles
(
    Id   BIGSERIAL PRIMARY KEY,
    Type SMALLINT    NOT NULL,
    Name VARCHAR(70) NOT NULL
);

CREATE TABLE Permissions
(
    Id   BIGSERIAL PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Type SMALLINT    NOT NULL
);

CREATE TABLE RolePermissions
(
    RoleId       BIGINT NOT NULL,
    PermissionId BIGINT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId)
);

ALTER TABLE RolePermissions
    ADD CONSTRAINT FK_RolePermission_Role FOREIGN KEY (RoleId) REFERENCES Roles (Id) ON DELETE CASCADE;

ALTER TABLE RolePermissions
    ADD CONSTRAINT FK_RolePermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions (Id) ON DELETE CASCADE;

CREATE TABLE ChatUserPermissions
(
    ChatUserId   BIGINT NOT NULL,
    PermissionId BIGINT NOT NULL,
    PRIMARY KEY (ChatUserId, PermissionId)
);

ALTER TABLE ChatUserPermissions
    ADD CONSTRAINT FK_ChatUserPermission_ChatUser FOREIGN KEY (ChatUserId) REFERENCES ChatUsers (Id) ON DELETE CASCADE;

ALTER TABLE ChatUserPermissions
    ADD CONSTRAINT FK_ChatUserPermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions (Id) ON DELETE CASCADE;

CREATE TABLE ChatUserRoles
(
    ChatUserId BIGINT NOT NULL,
    RoleId     BIGINT NOT NULL,
    PRIMARY KEY (ChatUserId, RoleId)
);

ALTER TABLE ChatUserRoles
    ADD CONSTRAINT FK_ChatUserRole_ChatUser FOREIGN KEY (ChatUserId) REFERENCES ChatUsers (Id) ON DELETE CASCADE;

ALTER TABLE ChatUserRoles
    ADD CONSTRAINT FK_ChatUserRole_Role FOREIGN KEY (RoleId) REFERENCES Roles (Id) ON DELETE CASCADE;

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
    ADD CONSTRAINT FK_MessageFile_Message FOREIGN KEY (MessageId) REFERENCES Messages (Id) ON DELETE CASCADE;

ALTER TABLE MessageFiles
    ADD CONSTRAINT FK_MessageFile_File FOREIGN KEY (FileId) REFERENCES Files (Id) ON DELETE CASCADE;
