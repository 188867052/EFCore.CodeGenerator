USE [master]
GO
/****** Object:  Database [Scaffolding]    Script Date: 2020/4/11 15:32:06 ******/
CREATE DATABASE [Scaffolding]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Scaffolding', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Scaffolding.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Scaffolding_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Scaffolding_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Scaffolding] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Scaffolding].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Scaffolding] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Scaffolding] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Scaffolding] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Scaffolding] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Scaffolding] SET ARITHABORT OFF 
GO
ALTER DATABASE [Scaffolding] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Scaffolding] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Scaffolding] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Scaffolding] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Scaffolding] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Scaffolding] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Scaffolding] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Scaffolding] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Scaffolding] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Scaffolding] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Scaffolding] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Scaffolding] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Scaffolding] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Scaffolding] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Scaffolding] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Scaffolding] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Scaffolding] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Scaffolding] SET RECOVERY FULL 
GO
ALTER DATABASE [Scaffolding] SET  MULTI_USER 
GO
ALTER DATABASE [Scaffolding] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Scaffolding] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Scaffolding] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Scaffolding] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Scaffolding] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Scaffolding] SET QUERY_STORE = OFF
GO
USE [Scaffolding]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Identifier] [uniqueidentifier] NOT NULL,
	[Message] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTimeTicks] [bigint] NULL,
	[Url] [nvarchar](100) NULL,
 CONSTRAINT [PK_log] PRIMARY KEY CLUSTERED 
(
	[Identifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[v_log]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_log]
AS
SELECT dbo.[log].*,newid() as new_id
FROM   dbo.[log]
GO
/****** Object:  Table [dbo].[Class]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Class](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[HeadTeacherId] [int] NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
	[GradeId] [int] NULL,
	[Location] [nchar](10) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_class] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Course]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Course](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nchar](10) NULL,
	[TeacherId] [nchar](10) NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
 CONSTRAINT [PK_course] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseScore]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseScore](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Score] [int] NULL,
	[StudentId] [int] NULL,
	[CourseId] [int] NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_score] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Grade]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Grade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Grade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nchar](10) NULL,
	[Sex] [nchar](10) NULL,
	[ClassId] [int] NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
	[Address] [nchar](10) NULL,
	[Mobile] [nchar](10) NULL,
	[IsDeleted] [nchar](10) NOT NULL,
 CONSTRAINT [PK_person] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teacher]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teacher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Sex] [nvarchar](50) NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_teacher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeacherCourseMapping]    Script Date: 2020/4/11 15:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeacherCourseMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NULL,
	[TeacherId] [int] NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
 CONSTRAINT [PK_teacher_course_mapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF__class__update_ti__440B1D61]  DEFAULT (NULL) FOR [UpdateTime]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF__course__create_t__44FF419A]  DEFAULT (NULL) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF__course__update_t__45F365D3]  DEFAULT (NULL) FOR [UpdateTime]
GO
ALTER TABLE [dbo].[CourseScore] ADD  DEFAULT (NULL) FOR [CreateTime]
GO
ALTER TABLE [dbo].[CourseScore] ADD  DEFAULT (NULL) FOR [UpdateTime]
GO
ALTER TABLE [dbo].[Log] ADD  CONSTRAINT [DF_log_identifier]  DEFAULT (newid()) FOR [Identifier]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF__student__create___60A75C0F]  DEFAULT (NULL) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF__student__update___619B8048]  DEFAULT (NULL) FOR [UpdateTime]
GO
ALTER TABLE [dbo].[Teacher] ADD  DEFAULT (NULL) FOR [CreateTime]
GO
ALTER TABLE [dbo].[Teacher] ADD  DEFAULT (NULL) FOR [UpdateTime]
GO
ALTER TABLE [dbo].[TeacherCourseMapping] ADD  DEFAULT (NULL) FOR [CreateTime]
GO
ALTER TABLE [dbo].[TeacherCourseMapping] ADD  DEFAULT (NULL) FOR [UpdateTime]
GO
ALTER TABLE [dbo].[CourseScore]  WITH CHECK ADD  CONSTRAINT [FK_course_score_course] FOREIGN KEY([Id])
REFERENCES [dbo].[Course] ([Id])
GO
ALTER TABLE [dbo].[CourseScore] CHECK CONSTRAINT [FK_course_score_course]
GO
ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK_student_class] FOREIGN KEY([ClassId])
REFERENCES [dbo].[Class] ([Id])
GO
ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK_student_class]
GO
ALTER TABLE [dbo].[TeacherCourseMapping]  WITH CHECK ADD  CONSTRAINT [FK_course_id] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Course] ([Id])
GO
ALTER TABLE [dbo].[TeacherCourseMapping] CHECK CONSTRAINT [FK_course_id]
GO
ALTER TABLE [dbo].[TeacherCourseMapping]  WITH CHECK ADD  CONSTRAINT [FK_teacher_id] FOREIGN KEY([TeacherId])
REFERENCES [dbo].[Teacher] ([Id])
GO
ALTER TABLE [dbo].[TeacherCourseMapping] CHECK CONSTRAINT [FK_teacher_id]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Grade', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Grade', @level2type=N'COLUMN',@level2name=N'Name'
GO
USE [master]
GO
ALTER DATABASE [Scaffolding] SET  READ_WRITE 
GO
