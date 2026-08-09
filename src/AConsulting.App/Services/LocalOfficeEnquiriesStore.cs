// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Human Centric Works, Hospet

using Microsoft.Data.Sqlite;

namespace AConsulting.App.Services;

/// <summary>
/// Office enquiry / go-no-go register in firm.db (AConsulting-owned).
/// </summary>
public sealed class LocalOfficeEnquiriesStore : IDisposable
{
    readonly SqliteConnection _con;

    public LocalOfficeEnquiriesStore(string firmDbPath)
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

    public static string DefaultFirmDbPath() => LocalEngagementsStore.DefaultFirmDbPath();

    void EnsureSchema()
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS local_office_enquiries(
              enquiry_id TEXT PRIMARY KEY,
              subject TEXT NOT NULL,
              client_name TEXT NOT NULL DEFAULT '',
              decision TEXT NOT NULL DEFAULT 'DRAFT',
              notes TEXT NOT NULL DEFAULT '',
              publish_state TEXT NOT NULL DEFAULT 'LOCAL',
              updated_at TEXT NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public void Upsert(
        string enquiryId,
        string subject,
        string clientName,
        string decision,
        string notes,
        string publishState)
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            INSERT INTO local_office_enquiries(
              enquiry_id, subject, client_name, decision, notes, publish_state, updated_at)
            VALUES($id,$s,$c,$d,$n,$ps,$u)
            ON CONFLICT(enquiry_id) DO UPDATE SET
              subject=excluded.subject,
              client_name=excluded.client_name,
              decision=excluded.decision,
              notes=excluded.notes,
              publish_state=excluded.publish_state,
              updated_at=excluded.updated_at
            """;
        cmd.Parameters.AddWithValue("$id", enquiryId);
        cmd.Parameters.AddWithValue("$s", subject);
        cmd.Parameters.AddWithValue("$c", clientName);
        cmd.Parameters.AddWithValue("$d", decision);
        cmd.Parameters.AddWithValue("$n", notes);
        cmd.Parameters.AddWithValue("$ps", publishState);
        cmd.Parameters.AddWithValue("$u", DateTime.UtcNow.ToString("O"));
        cmd.ExecuteNonQuery();
    }

    public LocalOfficeEnquiry? Get(string enquiryId)
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            SELECT enquiry_id, subject, client_name, decision, notes, publish_state
            FROM local_office_enquiries WHERE enquiry_id=$id
            """;
        cmd.Parameters.AddWithValue("$id", enquiryId);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return Read(r);
    }

    public IReadOnlyList<LocalOfficeEnquiry> List()
    {
        using var cmd = _con.CreateCommand();
        cmd.CommandText = """
            SELECT enquiry_id, subject, client_name, decision, notes, publish_state
            FROM local_office_enquiries ORDER BY updated_at DESC LIMIT 200
            """;
        using var r = cmd.ExecuteReader();
        var list = new List<LocalOfficeEnquiry>();
        while (r.Read()) list.Add(Read(r));
        return list;
    }

    static LocalOfficeEnquiry Read(SqliteDataReader r) => new(
        r.GetString(0),
        r.GetString(1),
        r.GetString(2),
        r.GetString(3),
        r.GetString(4),
        r.GetString(5));

    public void Dispose() => _con.Dispose();
}

public sealed record LocalOfficeEnquiry(
    string EnquiryId,
    string Subject,
    string ClientName,
    string Decision,
    string Notes,
    string PublishState);
