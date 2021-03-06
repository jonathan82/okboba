/*************************************************************************
*
* Clears the database of all user generated data (Profiles and related data):
*	Answers
*   ProfileTexts
*	ProfileDetails
*	Conversations, Messages, ConversationMaps
*	Profiles
*
* The users table (AspNetUsers, AspNetUserClaims, AspNetUserLogins, AspNetUserRoles)
* will be handled in a separate script and should be truncated first.
*
***************************************************************************/

USE [okboba]

SET XACT_ABORT ON

GO

BEGIN TRANSACTION ClearProfiles

/*************** Drop Indexes **********************/

-- Drop keys pointing to Profiles table
ALTER TABLE [dbo].[Answers] DROP CONSTRAINT [FK_dbo.UserAnswers_dbo.UserProfiles_UserProfileId]
ALTER TABLE [dbo].[AspNetUsers] DROP CONSTRAINT [FK_dbo.AspNetUsers_dbo.UserProfiles_UserProfile_Id]
ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_dbo.Messages_dbo.Profiles_FromProfileId]
ALTER TABLE [dbo].[ProfileDetails] DROP CONSTRAINT [FK_dbo.ProfileDetails_dbo.Profiles_ProfileId]
ALTER TABLE [dbo].[ProfileTexts] DROP CONSTRAINT [FK_dbo.ProfileTexts_dbo.Profiles_ProfileId]
ALTER TABLE [dbo].[ConversationMaps] DROP CONSTRAINT [FK_dbo.ConversationMaps_dbo.Profiles_Other]
ALTER TABLE [dbo].[ConversationMaps] DROP CONSTRAINT [FK_dbo.ConversationMaps_dbo.Profiles_ProfileId]

-- Drop keys pointing to Messages and related tables
ALTER TABLE [dbo].[ConversationMaps] DROP CONSTRAINT [FK_dbo.ConversationMaps_dbo.Messages_LastMessage_Id]
ALTER TABLE [dbo].[ConversationMaps] DROP CONSTRAINT [FK_dbo.ConversationMaps_dbo.Conversations_ConversationId]
ALTER TABLE [dbo].[Messages] DROP CONSTRAINT [FK_dbo.Messages_dbo.Conversations_ConversationId]


/*!!!!!!!!!!!!!!!!!!!!! Truncate tables !!!!!!!!!!!!!!!!!!!!!!!! */
truncate table Answers
truncate table ProfileTexts
truncate table ProfileDetails
truncate table Messages
truncate table ConversationMaps
truncate table Conversations
truncate table Profiles

-- Reset inital Seed values
DBCC CHECKIDENT (Messages, RESEED, 1) WITH NO_INFOMSGS
DBCC CHECKIDENT (Conversations, RESEED, 1) WITH NO_INFOMSGS
DBCC CHECKIDENT (Profiles, RESEED, 1) WITH NO_INFOMSGS


/********************* Create Indexes *****************************/

-- Create indexes pointing to Profiles table
ALTER TABLE [dbo].[Answers]  WITH NOCHECK ADD  CONSTRAINT [FK_dbo.UserAnswers_dbo.UserProfiles_UserProfileId] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profiles] ([Id]) 
ON DELETE CASCADE

ALTER TABLE [dbo].[Answers] CHECK CONSTRAINT [FK_dbo.UserAnswers_dbo.UserProfiles_UserProfileId]

ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUsers_dbo.UserProfiles_UserProfile_Id] FOREIGN KEY([Profile_Id])
REFERENCES [dbo].[Profiles] ([Id])

ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_dbo.AspNetUsers_dbo.UserProfiles_UserProfile_Id]

ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Messages_dbo.Profiles_FromProfileId] FOREIGN KEY([From])
REFERENCES [dbo].[Profiles] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_dbo.Messages_dbo.Profiles_FromProfileId]

ALTER TABLE [dbo].[ProfileDetails]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProfileDetails_dbo.Profiles_ProfileId] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profiles] ([Id])

ALTER TABLE [dbo].[ProfileDetails] CHECK CONSTRAINT [FK_dbo.ProfileDetails_dbo.Profiles_ProfileId]

ALTER TABLE [dbo].[ProfileTexts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProfileTexts_dbo.Profiles_ProfileId] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profiles] ([Id])

ALTER TABLE [dbo].[ProfileTexts] CHECK CONSTRAINT [FK_dbo.ProfileTexts_dbo.Profiles_ProfileId]

ALTER TABLE [dbo].[ConversationMaps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ConversationMaps_dbo.Profiles_Other] FOREIGN KEY([Other])
REFERENCES [dbo].[Profiles] ([Id])

ALTER TABLE [dbo].[ConversationMaps] CHECK CONSTRAINT [FK_dbo.ConversationMaps_dbo.Profiles_Other]

ALTER TABLE [dbo].[ConversationMaps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ConversationMaps_dbo.Profiles_ProfileId] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profiles] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[ConversationMaps] CHECK CONSTRAINT [FK_dbo.ConversationMaps_dbo.Profiles_ProfileId]

-- Create indexes related to Messages table (Messages, Conversations, ConversationMaps)
ALTER TABLE [dbo].[ConversationMaps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ConversationMaps_dbo.Messages_LastMessage_Id] FOREIGN KEY([LastMessage_Id])
REFERENCES [dbo].[Messages] ([Id])

ALTER TABLE [dbo].[ConversationMaps] CHECK CONSTRAINT [FK_dbo.ConversationMaps_dbo.Messages_LastMessage_Id]

ALTER TABLE [dbo].[ConversationMaps]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ConversationMaps_dbo.Conversations_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[ConversationMaps] CHECK CONSTRAINT [FK_dbo.ConversationMaps_dbo.Conversations_ConversationId]

ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Messages_dbo.Conversations_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_dbo.Messages_dbo.Conversations_ConversationId]

COMMIT TRANSACTION ClearProfiles