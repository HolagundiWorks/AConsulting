// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Human Centric Works, Hospet

using Aorms.Bridge;
using AConsulting.App.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AConsulting.App;

enum ShellModule
{
    Clients,
    Projects,
    Office,
    Tasks,
}

public sealed partial class MainWindow : Window
{
    readonly AormsBridge _bridge;
    readonly LocalEngagementsStore _engagements;
    readonly LocalOfficeEnquiriesStore _enquiries;
    readonly LocalClientsStore _clients;
    ShellModule _module = ShellModule.Projects;
    string? _selectedEngagementId;
    string? _selectedEnquiryId;
    string? _selectedClientId;

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = false;
        _bridge = AormsBridgeHost.CreateFromEnvironment();
        var dbPath = LocalEngagementsStore.DefaultFirmDbPath();
        _engagements = new LocalEngagementsStore(dbPath);
        _enquiries = new LocalOfficeEnquiriesStore(dbPath);
        _clients = new LocalClientsStore(dbPath);
        ShowModule(ShellModule.Projects);
        RefreshStatus("Ready.");
    }

    void ShowModule(ShellModule module)
    {
        _module = module;
        PanelClients.Visibility = module == ShellModule.Clients ? Visibility.Visible : Visibility.Collapsed;
        PanelProjects.Visibility = module == ShellModule.Projects ? Visibility.Visible : Visibility.Collapsed;
        PanelOffice.Visibility = module == ShellModule.Office ? Visibility.Visible : Visibility.Collapsed;
        PanelTasks.Visibility = module == ShellModule.Tasks ? Visibility.Visible : Visibility.Collapsed;

        StyleNav(NavClientsBtn, module == ShellModule.Clients);
        StyleNav(NavProjectsBtn, module == ShellModule.Projects);
        StyleNav(NavOfficeBtn, module == ShellModule.Office);
        StyleNav(NavTasksBtn, module == ShellModule.Tasks);

        DockImportBtn.Visibility = module == ShellModule.Projects
            ? Visibility.Visible
            : Visibility.Collapsed;

        DockCreateBtn.Content = module switch
        {
            ShellModule.Clients => "Save client",
            ShellModule.Projects => "Save engagement",
            ShellModule.Office => "Save enquiry",
            _ => "Save local",
        };
        DockCommitBtn.Content = module switch
        {
            ShellModule.Clients => "Publish client",
            ShellModule.Projects => "Publish status",
            ShellModule.Office => "Publish decision",
            _ => "Publish to hub",
        };
        TrayText.Text = $"AConsulting · {_module}";

        switch (module)
        {
            case ShellModule.Clients:
                ReloadClients();
                break;
            case ShellModule.Projects:
                ReloadEngagements();
                break;
            case ShellModule.Office:
                ReloadEnquiries();
                break;
            case ShellModule.Tasks:
                if (!string.IsNullOrEmpty(_selectedEngagementId) &&
                    string.IsNullOrWhiteSpace(TaskProjectBox.Text))
                    TaskProjectBox.Text = _selectedEngagementId;
                ReloadTasks();
                break;
        }
    }

    static void StyleNav(Button btn, bool active)
    {
        if (active)
        {
            btn.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Windows.UI.Color.FromArgb(255, 0xFF, 0x4F, 0x18));
            btn.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Windows.UI.Color.FromArgb(255, 255, 255, 255));
        }
        else
        {
            btn.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Windows.UI.Color.FromArgb(0, 0, 0, 0));
            btn.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Windows.UI.Color.FromArgb(255, 0x14, 0x15, 0x17));
        }
    }

    void NavClients_Click(object sender, RoutedEventArgs e) => ShowModule(ShellModule.Clients);
    void NavProjects_Click(object sender, RoutedEventArgs e) => ShowModule(ShellModule.Projects);
    void NavOffice_Click(object sender, RoutedEventArgs e) => ShowModule(ShellModule.Office);
    void NavTasks_Click(object sender, RoutedEventArgs e) => ShowModule(ShellModule.Tasks);

    void RefreshStatus(string? note = null)
    {
        var cfg = _bridge.HubConfigured();
        HubStatusText.Text =
            $"hub={cfg.HubUrl}  licenseApi={cfg.LicenseApiUrl}\n" +
            $"hasSyncToken={cfg.HasSyncToken}  syncReady={cfg.SyncReady}";
        if (!string.IsNullOrWhiteSpace(note))
            LogText.Text = note;
    }

    void Refresh_Click(object sender, RoutedEventArgs e) => RefreshStatus("Status refreshed.");

    async void Activate_Click(object sender, RoutedEventArgs e)
    {
        var key = LicenseKeyBox.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(key))
        {
            RefreshStatus("Enter a licence key first.");
            return;
        }
        try
        {
            LogText.Text = "Activating…";
            var grant = await _bridge.ActivateAsync(key);
            RefreshStatus($"Activate OK · syncToken length={grant.SyncToken?.Length ?? 0}");
        }
        catch (Exception ex)
        {
            RefreshStatus($"Activate failed: {ex.Message}");
        }
    }

    async void Flush_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LogText.Text = "Flushing…";
            var result = await _bridge.FlushAsync();
            if (result.SkippedReason is not null)
                RefreshStatus($"Flush skipped={result.SkippedReason}");
            else
                RefreshStatus($"Flush OK metaSent={result.MetaSent} artSent={result.ArtifactsSent}");
        }
        catch (Exception ex)
        {
            RefreshStatus($"Flush failed: {ex.Message}");
        }
    }

    void ReloadEngagements()
    {
        var rows = _engagements.List();
        if (rows.Count == 0)
        {
            EngListText.Text = "(empty — save an engagement, or Import from Connect)";
            return;
        }
        EngListText.Text = string.Join("\n", rows.Select(r =>
        {
            var mark = r.EngagementId == _selectedEngagementId ? ">" : " ";
            return $"{mark} {r.Code}  {r.Status}/{r.PublishState}  {r.Title}  [{r.EngagementId}]";
        }));
        if (_selectedEngagementId is null)
            _selectedEngagementId = rows[0].EngagementId;
    }

    void SelectNextEng_Click(object sender, RoutedEventArgs e)
    {
        var rows = _engagements.List();
        if (rows.Count == 0)
        {
            TrayText.Text = "No engagements yet.";
            return;
        }
        var idx = rows.ToList().FindIndex(r => r.EngagementId == _selectedEngagementId);
        idx = (idx + 1) % rows.Count;
        _selectedEngagementId = rows[idx].EngagementId;
        ReloadEngagements();
        TaskProjectBox.Text = _selectedEngagementId;
        TrayText.Text = $"Selected · {_selectedEngagementId}";
    }

    void SaveEngagement()
    {
        var title = EngTitleBox.Text?.Trim() ?? "";
        var code = EngCodeBox.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(code))
        {
            TrayText.Text = "Title and code required.";
            return;
        }
        var id = Guid.NewGuid().ToString("N")[..12];
        _engagements.Upsert(
            id,
            code,
            title,
            "ACTIVE",
            EngStageBox.Text?.Trim() ?? "",
            EngDisciplineBox.Text?.Trim() ?? "",
            "",
            "LOCAL");
        _selectedEngagementId = id;
        EngTitleBox.Text = "";
        EngCodeBox.Text = "";
        EngStageBox.Text = "";
        EngDisciplineBox.Text = "";
        ReloadEngagements();
        TrayText.Text = $"Saved engagement {id}";
    }

    async Task PublishEngagementStatusAsync()
    {
        var id = _selectedEngagementId;
        if (id is null)
        {
            TrayText.Text = "No engagement selected — save or Select next.";
            return;
        }
        var e = _engagements.Get(id);
        if (e is null)
        {
            TrayText.Text = "Engagement not found.";
            return;
        }
        try
        {
            _bridge.EnqueueMeta("engagementStatus", e.EngagementId, new Dictionary<string, object?>
            {
                ["engagementId"] = e.EngagementId,
                ["code"] = e.Code,
                ["title"] = e.Title,
                ["status"] = e.Status,
                ["stage"] = e.Stage,
                ["discipline"] = e.Discipline,
                ["updatedAt"] = DateTime.UtcNow.ToString("O"),
            });
            _engagements.Upsert(e.EngagementId, e.Code, e.Title, e.Status, e.Stage, e.Discipline, e.Notes, "QUEUED");
            var result = await _bridge.FlushAsync();
            if (result.SkippedReason is not null)
            {
                TrayText.Text =
                    $"Queued engagementStatus for {e.Code}; flush skipped={result.SkippedReason} — Activate first.";
                LogText.Text = $"Flush skipped={result.SkippedReason}";
            }
            else
            {
                _engagements.Upsert(e.EngagementId, e.Code, e.Title, e.Status, e.Stage, e.Discipline, e.Notes, "PUBLISHED");
                TrayText.Text = $"Published status · {e.Code} · metaSent={result.MetaSent}";
                LogText.Text = $"engagementStatus OK · {e.EngagementId}";
            }
            ReloadEngagements();
        }
        catch (Exception ex)
        {
            TrayText.Text = $"Publish failed: {ex.Message}";
        }
    }

    void ImportCatalog_Click(object sender, RoutedEventArgs e)
    {
        var catalog = ConnectCatalog.List();
        if (catalog.Count == 0)
        {
            var note =
                "Connect catalog empty — open AORMS Connect and sync projects, then retry. " +
                @"Expected: %LocalAppData%\AORMS-Connect\catalog.json";
            CatalogImportNote.Text = note;
            TrayText.Text = "Connect catalog empty.";
            LogText.Text = note;
            return;
        }
        var n = 0;
        var skipped = 0;
        foreach (var c in catalog)
        {
            if (string.IsNullOrWhiteSpace(c.Id)) continue;
            if (_engagements.Get(c.Id) is not null)
            {
                skipped++;
                continue;
            }
            _engagements.Upsert(
                c.Id,
                string.IsNullOrWhiteSpace(c.Ref) ? c.Id[..Math.Min(8, c.Id.Length)] : c.Ref,
                string.IsNullOrWhiteSpace(c.Title) ? c.Ref : c.Title,
                string.IsNullOrWhiteSpace(c.Status) ? "ACTIVE" : c.Status,
                "",
                "",
                "Imported from AORMS Connect",
                "LOCAL");
            n++;
        }
        if (_selectedEngagementId is null)
            _selectedEngagementId = _engagements.List().FirstOrDefault()?.EngagementId;
        ReloadEngagements();
        var status =
            n == 0
                ? $"Import complete — 0 new ({skipped} already local, {catalog.Count} in Connect)."
                : $"Imported {n} from Connect ({skipped} skipped). Select next, then Publish status.";
        CatalogImportNote.Text = status;
        TrayText.Text = n == 0 ? "No new Connect projects." : $"Imported {n} engagement(s).";
        LogText.Text = status;
        if (_module != ShellModule.Projects)
            ShowModule(ShellModule.Projects);
    }

    void ReloadClients()
    {
        var rows = _clients.List();
        if (rows.Count == 0)
        {
            ClientListText.Text = "(empty — save a client)";
            return;
        }
        ClientListText.Text = string.Join("\n", rows.Select(r =>
        {
            var mark = r.ClientId == _selectedClientId ? ">" : " ";
            return $"{mark} {r.PublishState}  {r.Name}  ·  {r.Contact}  [{r.ClientId}]";
        }));
        if (_selectedClientId is null)
            _selectedClientId = rows[0].ClientId;
    }

    void SelectNextClient_Click(object sender, RoutedEventArgs e)
    {
        var rows = _clients.List();
        if (rows.Count == 0)
        {
            TrayText.Text = "No clients yet.";
            return;
        }
        var idx = rows.ToList().FindIndex(r => r.ClientId == _selectedClientId);
        idx = (idx + 1) % rows.Count;
        _selectedClientId = rows[idx].ClientId;
        var cur = rows[idx];
        ClientNameBox.Text = cur.Name;
        ClientContactBox.Text = cur.Contact;
        ClientEmailBox.Text = cur.Email;
        ClientNotesBox.Text = cur.Notes;
        ReloadClients();
        TrayText.Text = $"Selected client · {_selectedClientId}";
    }

    void SaveClient()
    {
        var name = ClientNameBox.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(name))
        {
            TrayText.Text = "Client name required.";
            return;
        }
        var existing = _selectedClientId is null ? null : _clients.Get(_selectedClientId);
        var updating = existing is not null &&
            string.Equals(name, existing.Name, StringComparison.Ordinal);
        var id = updating ? existing!.ClientId : Guid.NewGuid().ToString("N")[..12];
        var publishState = updating ? existing!.PublishState : "LOCAL";
        _clients.Upsert(
            id,
            name,
            ClientContactBox.Text?.Trim() ?? "",
            ClientEmailBox.Text?.Trim() ?? "",
            ClientNotesBox.Text ?? "",
            publishState);
        _selectedClientId = id;
        ClientNameBox.Text = "";
        ClientContactBox.Text = "";
        ClientEmailBox.Text = "";
        ClientNotesBox.Text = "";
        ReloadClients();
        TrayText.Text = $"Saved client {id}";
    }

    async Task PublishClientAsync()
    {
        var id = _selectedClientId;
        if (id is null)
        {
            TrayText.Text = "No client selected — save or Select next.";
            return;
        }
        var row = _clients.Get(id);
        if (row is null)
        {
            TrayText.Text = "Client not found.";
            return;
        }
        try
        {
            _bridge.EnqueueMeta("clientStatus", row.ClientId, new Dictionary<string, object?>
            {
                ["clientId"] = row.ClientId,
                ["name"] = row.Name,
                ["contact"] = row.Contact,
                ["email"] = row.Email,
                ["updatedAt"] = DateTime.UtcNow.ToString("O"),
            });
            _clients.Upsert(row.ClientId, row.Name, row.Contact, row.Email, row.Notes, "QUEUED");
            var result = await _bridge.FlushAsync();
            if (result.SkippedReason is not null)
            {
                TrayText.Text =
                    $"Queued clientStatus; flush skipped={result.SkippedReason} — Activate first.";
                LogText.Text = $"Flush skipped={result.SkippedReason}";
            }
            else
            {
                _clients.Upsert(row.ClientId, row.Name, row.Contact, row.Email, row.Notes, "PUBLISHED");
                TrayText.Text = $"Published client · {row.Name}";
                LogText.Text = $"clientStatus OK · {row.ClientId}";
            }
            ReloadClients();
        }
        catch (Exception ex)
        {
            TrayText.Text = $"Publish failed: {ex.Message}";
        }
    }

    void ReloadEnquiries()
    {
        var rows = _enquiries.List();
        if (rows.Count == 0)
        {
            EnqListText.Text = "(empty — save an enquiry with DRAFT / GO / NO_GO)";
            return;
        }
        EnqListText.Text = string.Join("\n", rows.Select(r =>
        {
            var mark = r.EnquiryId == _selectedEnquiryId ? ">" : " ";
            return $"{mark} {r.Decision}/{r.PublishState}  {r.Subject}  ·  {r.ClientName}  [{r.EnquiryId}]";
        }));
        if (_selectedEnquiryId is null)
            _selectedEnquiryId = rows[0].EnquiryId;
    }

    void SelectNextEnq_Click(object sender, RoutedEventArgs e)
    {
        var rows = _enquiries.List();
        if (rows.Count == 0)
        {
            TrayText.Text = "No enquiries yet.";
            return;
        }
        var idx = rows.ToList().FindIndex(r => r.EnquiryId == _selectedEnquiryId);
        idx = (idx + 1) % rows.Count;
        _selectedEnquiryId = rows[idx].EnquiryId;
        var cur = rows[idx];
        EnqSubjectBox.Text = cur.Subject;
        EnqClientBox.Text = cur.ClientName;
        EnqDecisionBox.Text = cur.Decision;
        EnqNotesBox.Text = cur.Notes;
        ReloadEnquiries();
        TrayText.Text = $"Selected enquiry · {_selectedEnquiryId}";
    }

    void SaveEnquiry()
    {
        var subject = EnqSubjectBox.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(subject))
        {
            TrayText.Text = "Subject required.";
            return;
        }
        var decision = string.IsNullOrWhiteSpace(EnqDecisionBox.Text)
            ? "DRAFT"
            : EnqDecisionBox.Text.Trim().ToUpperInvariant();
        if (decision is not ("DRAFT" or "GO" or "NO_GO"))
            decision = "DRAFT";

        // Update selected row when form was loaded via Select next; otherwise create.
        var existing = _selectedEnquiryId is null ? null : _enquiries.Get(_selectedEnquiryId);
        var updating = existing is not null &&
            string.Equals(EnqSubjectBox.Text?.Trim(), existing.Subject, StringComparison.Ordinal);
        var id = updating ? existing!.EnquiryId : Guid.NewGuid().ToString("N")[..12];
        var publishState = updating ? existing!.PublishState : "LOCAL";

        _enquiries.Upsert(
            id,
            subject,
            EnqClientBox.Text?.Trim() ?? "",
            decision,
            EnqNotesBox.Text ?? "",
            publishState);
        _selectedEnquiryId = id;
        EnqSubjectBox.Text = "";
        EnqClientBox.Text = "";
        EnqDecisionBox.Text = "";
        EnqNotesBox.Text = "";
        ReloadEnquiries();
        TrayText.Text = $"Saved enquiry {id} · {decision}";
    }

    async Task PublishEnquiryDecisionAsync()
    {
        var id = _selectedEnquiryId;
        if (id is null)
        {
            TrayText.Text = "No enquiry selected — save or Select next.";
            return;
        }
        var row = _enquiries.Get(id);
        if (row is null)
        {
            TrayText.Text = "Enquiry not found.";
            return;
        }
        if (row.Decision is "DRAFT")
        {
            TrayText.Text = "Set decision to GO or NO_GO before publish.";
            return;
        }
        try
        {
            _bridge.EnqueueMeta("officeEnquiry", row.EnquiryId, new Dictionary<string, object?>
            {
                ["enquiryId"] = row.EnquiryId,
                ["subject"] = row.Subject,
                ["clientName"] = row.ClientName,
                ["decision"] = row.Decision,
                ["updatedAt"] = DateTime.UtcNow.ToString("O"),
            });
            _enquiries.Upsert(row.EnquiryId, row.Subject, row.ClientName, row.Decision, row.Notes, "QUEUED");
            var result = await _bridge.FlushAsync();
            if (result.SkippedReason is not null)
            {
                TrayText.Text =
                    $"Queued officeEnquiry; flush skipped={result.SkippedReason} — Activate first.";
                LogText.Text = $"Flush skipped={result.SkippedReason}";
            }
            else
            {
                _enquiries.Upsert(row.EnquiryId, row.Subject, row.ClientName, row.Decision, row.Notes, "PUBLISHED");
                TrayText.Text = $"Published decision · {row.Decision} · {row.Subject}";
                LogText.Text = $"officeEnquiry OK · {row.EnquiryId}";
            }
            ReloadEnquiries();
        }
        catch (Exception ex)
        {
            TrayText.Text = $"Publish failed: {ex.Message}";
        }
    }

    void SaveTaskLocal()
    {
        var title = TaskTitleBox.Text?.Trim() ?? "";
        var projectId = ResolveTaskProjectId();
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(projectId))
        {
            TrayText.Text = "Task title and engagement id required.";
            return;
        }
        var taskId = Guid.NewGuid().ToString("N")[..12];
        _bridge.Db.UpsertLocalTask(taskId, projectId, title, "OPEN", "LOCAL");
        TaskTitleBox.Text = "";
        ReloadTasks();
        TrayText.Text = $"Saved local task {taskId}";
    }

    async Task PublishTaskAsync()
    {
        var title = TaskTitleBox.Text?.Trim() ?? "";
        var projectId = ResolveTaskProjectId();
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(projectId))
        {
            TrayText.Text = "Task title and engagement id required.";
            return;
        }
        var taskId = Guid.NewGuid().ToString("N")[..12];
        try
        {
            await _bridge.PublishOpsTaskAsync(projectId, taskId, title, "OPEN");
            TaskTitleBox.Text = "";
            ReloadTasks();
            TrayText.Text = $"Published task {taskId}";
        }
        catch (Exception ex)
        {
            TrayText.Text = $"Publish failed: {ex.Message}";
        }
    }

    string ResolveTaskProjectId()
    {
        var typed = TaskProjectBox.Text?.Trim() ?? "";
        if (!string.IsNullOrEmpty(typed)) return typed;
        return _selectedEngagementId ?? "";
    }

    void ReloadTasks()
    {
        var rows = _bridge.Db.ListLocalTasks();
        TaskListText.Text = rows.Count == 0
            ? "(no local tasks)"
            : string.Join("\n", rows.Select(r => $"{r.TaskId}  {r.Status}/{r.PublishState}  {r.Title}"));
    }

    void ClearForm_Click(object sender, RoutedEventArgs e)
    {
        switch (_module)
        {
            case ShellModule.Clients:
                ClientNameBox.Text = "";
                ClientContactBox.Text = "";
                ClientEmailBox.Text = "";
                ClientNotesBox.Text = "";
                break;
            case ShellModule.Projects:
                EngTitleBox.Text = "";
                EngCodeBox.Text = "";
                EngStageBox.Text = "";
                EngDisciplineBox.Text = "";
                break;
            case ShellModule.Office:
                EnqSubjectBox.Text = "";
                EnqClientBox.Text = "";
                EnqDecisionBox.Text = "";
                EnqNotesBox.Text = "";
                break;
            default:
                TaskTitleBox.Text = "";
                break;
        }
        TrayText.Text = "Form cleared.";
    }

    void DockCreate_Click(object sender, RoutedEventArgs e)
    {
        switch (_module)
        {
            case ShellModule.Clients:
                SaveClient();
                break;
            case ShellModule.Projects:
                SaveEngagement();
                break;
            case ShellModule.Office:
                SaveEnquiry();
                break;
            default:
                SaveTaskLocal();
                break;
        }
    }

    void DockReload_Click(object sender, RoutedEventArgs e)
    {
        switch (_module)
        {
            case ShellModule.Clients:
                ReloadClients();
                break;
            case ShellModule.Projects:
                ReloadEngagements();
                break;
            case ShellModule.Office:
                ReloadEnquiries();
                break;
            default:
                ReloadTasks();
                break;
        }
        TrayText.Text = "Reloaded.";
    }

    async void DockCommit_Click(object sender, RoutedEventArgs e)
    {
        switch (_module)
        {
            case ShellModule.Clients:
                await PublishClientAsync();
                break;
            case ShellModule.Projects:
                await PublishEngagementStatusAsync();
                break;
            case ShellModule.Office:
                await PublishEnquiryDecisionAsync();
                break;
            default:
                await PublishTaskAsync();
                break;
        }
    }
}
