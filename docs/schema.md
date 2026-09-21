# Database Schema / ERD Notes

This project uses DB-first entities mapped from the existing SQL schema.

## Core Tables

### `tCompany`
- `Id` (PK)
- `Name`
- `CreatedOn`, `UpdatedOn`

### `tBusinessStudio`
- `Id` (PK)
- `CompanyId` (FK -> `tCompany.Id`)
- `Name`
- `CreatedOn`, `UpdatedOn`

### `tUser`
- `Id` (PK)
- `Name`
- `Email` (unique)
- `CreatedOn`, `UpdatedOn`

### `tPackage`
- `Id` (PK)
- `UserId` (FK -> `tUser.Id`)
- `BusinessStudioId` (FK -> `tBusinessStudio.Id`)
- `TotalCredits`
- `RemainingCredits`
- `ExpiryDate`
- `CreatedOn`, `UpdatedOn`

### `tTimetableSchedule`
- `Id` (PK)
- `BusinessStudioId` (FK -> `tBusinessStudio.Id`)
- `ClassName`
- `InstructorName`
- `StartTime`, `EndTime`
- `Capacity`
- `CreatedOn`, `UpdatedOn`

### `tBooking`
- `Id` (PK)
- `BookingNo` (unique)
- `UserId` (FK -> `tUser.Id`)
- `TimetableScheduleId` (FK -> `tTimetableSchedule.Id`)
- `PackageId` (FK -> `tPackage.Id`)
- `Status` (`Booked`, `Cancelled`)
- `BookedOn`, `CancelledOn`
- `CreatedOn`, `UpdatedOn`

### `tBookingWaitlist`
- `Id` (PK)
- `UserId` (FK -> `tUser.Id`)
- `TimetableScheduleId` (FK -> `tTimetableSchedule.Id`)
- `Status` (`Waiting`, `Promoted`, `Expired`)
- `JoinedAt`, `PromotedAt`
- `CreatedOn`, `UpdatedOn`

## Relationship Summary
- Company 1..* BusinessStudio
- BusinessStudio 1..* TimetableSchedule
- User 1..* Package
- TimetableSchedule 1..* Booking
- TimetableSchedule 1..* BookingWaitlist
- Package 1..* Booking

## Rule Mapping to Schema
- Package-business match: `tPackage.BusinessStudioId == tTimetableSchedule.BusinessStudioId`
- Slot control: count of `tBooking` with `Status='Booked'` per schedule vs `tTimetableSchedule.Capacity`
- Waitlist FIFO: ordered by `tBookingWaitlist.JoinedAt`
- Credits: mutate `tPackage.RemainingCredits` on booking/refund/promotion
