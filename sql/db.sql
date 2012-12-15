SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Party]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Party](
	[PartyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Party] PRIMARY KEY CLUSTERED 
(
	[PartyId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attachment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Attachment](
	[AttachmentId] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](1000) NOT NULL,
	[Body] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Order](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[Number] [nvarchar](50) NOT NULL,
	[BillToPartyId] [int] NULL,
	[SupplierPartyId] [int] NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AttachmentBridge]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AttachmentBridge](
	[ItemId] [int] NOT NULL,
	[AttachmentId] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Line]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Line](
	[LineId] [int] IDENTITY(1,1) NOT NULL,
	[Number] [nvarchar](50) NOT NULL,
	[OrderId] [int] NOT NULL,
 CONSTRAINT [PK_Line] PRIMARY KEY CLUSTERED 
(
	[LineId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Party]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Party] FOREIGN KEY([BillToPartyId])
REFERENCES [dbo].[Party] ([PartyId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Party1]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Party1] FOREIGN KEY([SupplierPartyId])
REFERENCES [dbo].[Party] ([PartyId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AttachmentBridge_Attachment]') AND parent_object_id = OBJECT_ID(N'[dbo].[AttachmentBridge]'))
ALTER TABLE [dbo].[AttachmentBridge]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentBridge_Attachment] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachment] ([AttachmentId])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Line_Order]') AND parent_object_id = OBJECT_ID(N'[dbo].[Line]'))
ALTER TABLE [dbo].[Line]  WITH CHECK ADD  CONSTRAINT [FK_Line_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([OrderId])
