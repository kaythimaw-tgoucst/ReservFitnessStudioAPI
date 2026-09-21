-- =========================================================
-- Database: FitnessStudio
-- =========================================================
CREATE DATABASE FitnessStudio;
GO
USE FitnessStudio;
GO

-- =========================================================
-- 1. tCompany
-- =========================================================
CREATE TABLE tCompany (
	Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	Name        NVARCHAR(150) NOT NULL,
	CreatedOn   DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn   DATETIME NULL
);
GO

-- =========================================================
-- 2. tBusinessStudio
-- =========================================================
CREATE TABLE tBusinessStudio (
	Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	CompanyId   UNIQUEIDENTIFIER NOT NULL,
	Name        NVARCHAR(150) NOT NULL,
	CreatedOn   DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn   DATETIME NULL,
	CONSTRAINT FK_BusinessStudio_Company FOREIGN KEY (CompanyId) REFERENCES tCompany(Id)
);
GO

-- =========================================================
-- 3. tTimetableSchedule
-- =========================================================
CREATE TABLE tTimetableSchedule (
	Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	BusinessStudioId    UNIQUEIDENTIFIER NOT NULL,
	ClassName           NVARCHAR(100) NOT NULL,
	InstructorName      NVARCHAR(100) NOT NULL,
	StartTime           DATETIME NOT NULL,
	EndTime             DATETIME NOT NULL,
	Capacity            INT NOT NULL,
	CreatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn           DATETIME NULL,
	CONSTRAINT FK_Schedule_BusinessStudio FOREIGN KEY (BusinessStudioId) REFERENCES tBusinessStudio(Id),
	CONSTRAINT CK_Schedule_Time CHECK (EndTime > StartTime),
	CONSTRAINT CK_Schedule_Capacity CHECK (Capacity >= 0)
);
GO

-- =========================================================
-- 4. tUser
-- =========================================================
CREATE TABLE tUser (
	Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	Name        NVARCHAR(100) NOT NULL,
	Email       NVARCHAR(255) NOT NULL,
	CreatedOn   DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn   DATETIME NULL,
	CONSTRAINT UQ_User_Email UNIQUE (Email)
);
GO

-- =========================================================
-- 5. tPackage
-- =========================================================
CREATE TABLE tPackage (
	Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	UserId              UNIQUEIDENTIFIER NOT NULL,
	BusinessStudioId    UNIQUEIDENTIFIER NOT NULL,
	TotalCredits        DECIMAL(10,2) NOT NULL,
	RemainingCredits    DECIMAL(10,2) NOT NULL,
	ExpiryDate          DATETIME NOT NULL,
	CreatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn           DATETIME NULL,
	CONSTRAINT FK_Package_User FOREIGN KEY (UserId) REFERENCES tUser(Id),
	CONSTRAINT FK_Package_BusinessStudio FOREIGN KEY (BusinessStudioId) REFERENCES tBusinessStudio(Id),
	CONSTRAINT CK_Package_RemainingLETotal CHECK (RemainingCredits <= TotalCredits),
	CONSTRAINT CK_Package_RemainingGEZero CHECK (RemainingCredits >= 0)
);
GO

-- =========================================================
-- 6. tNumberingFormat
--    Combined format + counter table used to generate
--    unique BookingNo / TransactionNo per Studio + Type + Month
-- =========================================================
CREATE TABLE tNumberingFormat (
	Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	BusinessStudioId    UNIQUEIDENTIFIER NOT NULL,
	VoucherType         NVARCHAR(20) NOT NULL,   -- 'Booking' or 'Transaction'
	Prefix              NVARCHAR(10) NOT NULL,   -- e.g. 'BK', 'TX'
	FormatPattern       NVARCHAR(50) NULL,       -- e.g. '{Prefix}-{YYYYMM}-{Seq:000000}'
	YearMonth           CHAR(6) NOT NULL,        -- e.g. '202609'
	LastNumber          INT NOT NULL DEFAULT 0,
	CreatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn           DATETIME NULL,
	CONSTRAINT FK_NumberingFormat_BusinessStudio FOREIGN KEY (BusinessStudioId) REFERENCES tBusinessStudio(Id),
	CONSTRAINT UQ_NumberingFormat_Scope UNIQUE (BusinessStudioId, VoucherType, YearMonth)
);
GO

-- =========================================================
-- 7. tBooking
-- =========================================================
CREATE TABLE tBooking (
	Id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	BookingNo               NVARCHAR(30) NOT NULL,   -- e.g. BK-202609-000123
	UserId                  UNIQUEIDENTIFIER NOT NULL,
	TimetableScheduleId     UNIQUEIDENTIFIER NOT NULL,
	PackageId               UNIQUEIDENTIFIER NOT NULL,
	Status                  NVARCHAR(50) NOT NULL,   -- 'Booked' / 'Cancelled'
	BookedOn                DATETIME NOT NULL DEFAULT GETDATE(),
	CancelledOn             DATETIME NULL,
	CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn               DATETIME NULL,
	CONSTRAINT UQ_Booking_BookingNo UNIQUE (BookingNo),
	CONSTRAINT FK_Booking_User FOREIGN KEY (UserId) REFERENCES tUser(Id),
	CONSTRAINT FK_Booking_Schedule FOREIGN KEY (TimetableScheduleId) REFERENCES tTimetableSchedule(Id),
	CONSTRAINT FK_Booking_Package FOREIGN KEY (PackageId) REFERENCES tPackage(Id),
	CONSTRAINT CK_Booking_Status CHECK (Status IN ('Booked', 'Cancelled'))
);
GO

-- =========================================================
-- 8. tBookingWaitlist
-- =========================================================
CREATE TABLE tBookingWaitlist (
	Id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	UserId                  UNIQUEIDENTIFIER NOT NULL,
	TimetableScheduleId     UNIQUEIDENTIFIER NOT NULL,
	Status                  NVARCHAR(20) NOT NULL,   -- Waiting/Promoted/Expired/Cancelled
	JoinedAt                DATETIME NOT NULL DEFAULT GETDATE(),
	PromotedAt              DATETIME NULL,
	CreatedOn               DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn               DATETIME NULL,
	CONSTRAINT FK_Waitlist_User FOREIGN KEY (UserId) REFERENCES tUser(Id),
	CONSTRAINT FK_Waitlist_Schedule FOREIGN KEY (TimetableScheduleId) REFERENCES tTimetableSchedule(Id),
	CONSTRAINT CK_Waitlist_Status CHECK (Status IN ('Waiting', 'Promoted', 'Expired', 'Cancelled'))
);
GO

-- Helpful index for FIFO promotion order
CREATE INDEX IX_Waitlist_Schedule_JoinedAt
	ON tBookingWaitlist (TimetableScheduleId, JoinedAt ASC);
GO

-- =========================================================
-- 9. tTransaction
-- =========================================================
CREATE TABLE tTransaction (
	Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	TransactionNo       NVARCHAR(30) NOT NULL,   -- e.g. TX-202609-000456
	BookingId           UNIQUEIDENTIFIER NOT NULL,
	BusinessStudioId    UNIQUEIDENTIFIER NOT NULL,
	TotalCredits        DECIMAL(10,2) NOT NULL,
	RemainingCredits    DECIMAL(10,2) NOT NULL,
	ExpiryDate          DATETIME NOT NULL,
	CreatedOn           DATETIME NOT NULL DEFAULT GETDATE(),
	UpdatedOn           DATETIME NULL,
	CONSTRAINT UQ_Transaction_TransactionNo UNIQUE (TransactionNo),
	CONSTRAINT FK_Transaction_Booking FOREIGN KEY (BookingId) REFERENCES tBooking(Id),
	CONSTRAINT FK_Transaction_BusinessStudio FOREIGN KEY (BusinessStudioId) REFERENCES tBusinessStudio(Id),
	CONSTRAINT CK_Transaction_RemainingLETotal CHECK (RemainingCredits <= TotalCredits),
	CONSTRAINT CK_Transaction_RemainingGEZero CHECK (RemainingCredits >= 0)
);
GO
-- =========================================================
-- 10. tUserBusinessStudioMembership
-- =========================================================
CREATE TABLE [dbo].[tUserBusinessStudioMembership]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[BusinessStudioId] UNIQUEIDENTIFIER NOT NULL,
	[Role] NVARCHAR(100) NULL,
	[IsActive] BIT NOT NULL CONSTRAINT [DF_tUserBusinessStudioMembership_IsActive] DEFAULT (1),
	[CreatedOn] DATETIME NOT NULL CONSTRAINT [DF_tUserBusinessStudioMembership_CreatedOn] DEFAULT (GETDATE()),

	CONSTRAINT [PK_tUserBusinessStudioMembership]
		PRIMARY KEY ([UserId], [BusinessStudioId]),

	CONSTRAINT [FK_UserBusinessStudioMembership_User]
		FOREIGN KEY ([UserId]) REFERENCES [dbo].[tUser]([Id]),

	CONSTRAINT [FK_UserBusinessStudioMembership_BusinessStudio]
		FOREIGN KEY ([BusinessStudioId]) REFERENCES [dbo].[tBusinessStudio]([Id])
);
GO

-- =========================================================
-- Helpful supporting indexes
-- =========================================================
CREATE INDEX IX_Booking_User            ON tBooking (UserId);
CREATE INDEX IX_Booking_Schedule        ON tBooking (TimetableScheduleId);
CREATE INDEX IX_Package_User            ON tPackage (UserId);
CREATE INDEX IX_Package_BusinessStudio  ON tPackage (BusinessStudioId);
CREATE INDEX IX_Schedule_BusinessStudio ON tTimetableSchedule (BusinessStudioId);
CREATE INDEX IX_tUserBusinessStudioMembership_BusinessStudioId_UserId ON tUserBusinessStudioMembership ([BusinessStudioId], [UserId]);
GO
