IF COL_LENGTH('log', 'url') IS NOT NULL    
    PRINT N'存在'    
ELSE    
    alter table log add url nvarchar(100) null 