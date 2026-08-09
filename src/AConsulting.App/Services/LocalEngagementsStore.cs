// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Human Centric Works, Hospet

using Microsoft.Data.Sqlite;

namespace AConsulting.App.Services;

/// <summary>
/// Practice engagements in firm.db (AConsulting-owned; does not alter Aorms.Bridge schema).
/// </summary>
public sealed class LocalEngagementsStore : IDisposable
{
    readonly SqliteConnection _con;

    public LocalEngagementsStore(string firmDbPath)
    {
        var dir = Path.GetDirectoryName(firmDbPath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        SQLitePCL.Batteries_V2.Init();
        _con = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = firmDbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString());
        _con.Open();
        EnsureSchema();
    }

    public static string DefaultFirmDbPath() =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AConsulting",
            "firm.db");

    void EnsureSchema()
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS local_engagements(
              engagement_id TEXT PRIMARY KEY,
              code TEXT NOT NULL,
              title TEXT NOT NULL,
              status TEXT NOT NULL DEFAULT 'ACTIVE',
              stage TEXT NOT NULL DEFAULT '',
              discipline TEXT NOT NULL DEFAULT '',
              notes TEXT NOT NULL DEFAULT '',
              publish_state TEXT NOT NULL DEFAULT 'LOCAL',
              updated_at TEXT NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public void Upsert(
        string engagementId,
        string code,
        string title,
        string status,
        string stage,
        string discipline,
        string notes,
        string publishState)
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            INSERT INTO local_engagements(
              engagement_id, code, title, status, stage, discipline, notes, publish_state, updated_at)
            VALUES($id,$c,$t,$s,$st,$d,$n,$ps,$u)
            ON CONFLICT(engagement_id) DO UPDATE SET
              code=excluded.code,
              title=excluded.title,
              status=excluded.status,
              stage=excluded.stage,
              discipline=excluded.discipline,
              notes=excluded.notes,
              publish_state=excluded.publish_state,
              updated_at=excluded.updated_at
            """;
        cmd.Parameters.AddWithValue("$id", engagementId);
        cmd.Parameters.AddWithValue("$c", code);
        cmd.Parameters.AddWithValue("$t", title);
        cmd.Parameters.AddWithValue("$s", status);
        cmd.Parameters.AddWithValue("$st", stage);
        cmd.Parameters.AddWithValue("$d", discipline);
        cmd.Parameters.AddWithValue("$n", notes);
        cmd.Parameters.AddWithValue("$ps", publishState);
        cmd.Parameters.AddWithValue("$u", DateTime.UtcNow.ToString("O"));
        cmd.ExecuteNonQuery();
    }

    public LocalEngagement? Get(string engagementId)
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            SELECT engagement_id, code, title, status, stage, discipline, notes, publish_state
            FROM local_engagements WHERE engagement_id=$id
            """;
        cmd.Parameters.AddWithValue("$id", engagementId);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return Read(r);
    }

    public IReadOnlyList<LocalEngagement> List()
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            SELECT engagement_id, code, title, status, stage, discipline, notes, publish_state
            FROM local_engagements ORDER BY updated_at DESC LIMIT 200
            """;
        using var r = cmd.ExecuteReader();
        var list = new List<LocalEngagement>();
        while (r.Read()) list.Add(Read(r));
        return list;
    }

    static LocalEngagement Read(SqliteDataReader r) => new(
        r.GetString(0),
        r.GetString(1),
        r.GetString(2),
        r.GetString(3),
        r.GetString(4),
        r.GetString(5),
        r.GetString(6),
        r.GetString(7));

    public void Dispose() => _con.Dispose();
}

public sealed record LocalEngagement(
    string EngagementId,
    string Code,
    string Title,
    string Status,
    string Stage,
    string Discipline,
    string Notes,
    string PublishState);
