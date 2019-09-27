DROP TABLE [student]
CREATE TABLE [dbo].[student](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nchar](10) NULL,
	[sex] [nchar](10) NULL,
	[class_id] [int] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
	[address] [nchar](10) NULL,
	[mobile] [nchar](10) NULL,
	[is_deleted] [nchar](10) NOT NULL,
 CONSTRAINT [PK_person] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[student] ADD  CONSTRAINT [DF__student__create___60A75C0F]  DEFAULT (NULL) FOR [create_time]
ALTER TABLE [dbo].[student] ADD  CONSTRAINT [DF__student__update___619B8048]  DEFAULT (NULL) FOR [update_time]
ALTER TABLE [dbo].[student]  WITH CHECK ADD  CONSTRAINT [FK_student_class] FOREIGN KEY([class_id])
REFERENCES [dbo].[class] ([id])
ALTER TABLE [dbo].[student] CHECK CONSTRAINT [FK_student_class]


