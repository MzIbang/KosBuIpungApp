// ===== Views/AdminDashboardForm.cs =====
// (File ini sangat panjang, pastikan Anda menyalin semuanya)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KosBuIpungApp.Enums;
using KosBuIpungApp.Models;
using KosBuIpungApp.Services;

namespace KosBuIpungApp.Views
{
    public class AdminDashboardForm : Form
    {
        private TabControl mainTabControl;

        // Controls untuk Dashboard Tab
        private Label lblTotalKamar, lblKamarTerisi, lblKamarKosong, lblTotalPenghuni;

        // Controls untuk Kelola Kamar Tab
        private DataGridView dgvRooms;
        private TextBox txtRoomNumber, txtRoomFacilities;
        private ComboBox cmbRoomType, cmbRoomStatus;
        private NumericUpDown numRoomPrice;
        private Button btnAddRoom, btnUpdateRoom, btnDeleteRoom;
        private Room selectedRoom = null;

        // Controls untuk Kelola Penghuni Tab
        private DataGridView dgvTenants;
        private Button btnRemoveTenant;

        // Controls untuk Kelola Tagihan Tab
        private DataGridView dgvBillings;
        private Button btnCreateBilling, btnVerifyPayment, btnSetUnpaid;


        public AdminDashboardForm()
        {
            InitializeComponent();
            LoadDashboardData();
            LoadRoomData();
            LoadTenantData();
            LoadBillingData();
        }

        private void InitializeComponent()
        {
            this.Text = $"Admin Dashboard - Selamat Datang, {AuthService.CurrentUser.FullName}";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosed += (s, e) => Application.Exit();

            // Main TabControl
            mainTabControl = new TabControl();
            mainTabControl.Dock = DockStyle.Fill;
            this.Controls.Add(mainTabControl);

            // Buat Tab Pages
            var tabPageDashboard = new TabPage("Dashboard");
            var tabPageRooms = new TabPage("Kelola Kamar & Tipe");
            var tabPageTenants = new TabPage("Kelola Penghuni");
            var tabPageBillings = new TabPage("Kelola Tagihan");

            // Tambah Tab Pages ke Control
            mainTabControl.TabPages.Add(tabPageDashboard);
            mainTabControl.TabPages.Add(tabPageRooms);
            mainTabControl.TabPages.Add(tabPageTenants);
            mainTabControl.TabPages.Add(tabPageBillings);

            // Inisialisasi konten setiap tab
            InitializeDashboardTab(tabPageDashboard);
            InitializeRoomsTab(tabPageRooms);
            InitializeTenantsTab(tabPageTenants);
            InitializeBillingsTab(tabPageBillings);
        }

        #region Dashboard Tab
        private void InitializeDashboardTab(TabPage page)
        {
            page.BackColor = Color.White;
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };
            page.Controls.Add(panel);

            var title = new Label { Text = "Statistik Real-Time Kos Bu Ipung", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            panel.Controls.Add(title);

            lblTotalKamar = CreateStatLabel("Total Kamar:", "0");
            lblKamarTerisi = CreateStatLabel("Kamar Terisi:", "0");
            lblKamarKosong = CreateStatLabel("Kamar Kosong:", "0");
            lblTotalPenghuni = CreateStatLabel("Total Penghuni Aktif:", "0");

            panel.Controls.Add(lblTotalKamar);
            panel.Controls.Add(lblKamarTerisi);
            panel.Controls.Add(lblKamarKosong);
            panel.Controls.Add(lblTotalPenghuni);
        }

        private Label CreateStatLabel(string text, string value)
        {
            var label = new Label { Font = new Font("Segoe UI", 12), AutoSize = true, Margin = new Padding(10, 10, 10, 10) };
            label.Text = $"{text} {value}";
            return label;
        }

        private void LoadDashboardData()
        {
            int totalKamar = DataService.Rooms.Count;
            int kamarTerisi = DataService.Rooms.Count(r => r.Status == RoomStatus.Terisi);
            int totalPenghuni = DataService.Tenants.Count(t => t.CheckOutDate == null);

            lblTotalKamar.Text = $"Total Kamar: {totalKamar}";
            lblKamarTerisi.Text = $"Kamar Terisi: {kamarTerisi}";
            lblKamarKosong.Text = $"Kamar Kosong: {totalKamar - kamarTerisi}";
            lblTotalPenghuni.Text = $"Total Penghuni Aktif: {totalPenghuni}";
        }
        #endregion

        #region Kelola Kamar Tab
        private void InitializeRoomsTab(TabPage page)
        {
            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 250 };
            page.Controls.Add(splitContainer);

            // Top Panel: DataGridView
            dgvRooms = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false };
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "No. Kamar", DataPropertyName = "RoomNumber", Width = 100 });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipe Kamar", DataPropertyName = "TypeName", Width = 120 });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Harga", DataPropertyName = "Price", DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" }, Width = 120 });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 100 });
            dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fasilitas", DataPropertyName = "Facilities", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvRooms.SelectionChanged += DgvRooms_SelectionChanged;
            splitContainer.Panel1.Controls.Add(dgvRooms);

            // Bottom Panel: Form
            var formPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            splitContainer.Panel2.Controls.Add(formPanel);

            // Form controls
            int y = 10;
            formPanel.Controls.Add(new Label { Text = "No. Kamar:", Location = new Point(10, y) });
            txtRoomNumber = new TextBox { Location = new Point(100, y), Size = new Size(150, 20) };
            formPanel.Controls.Add(txtRoomNumber);

            formPanel.Controls.Add(new Label { Text = "Status:", Location = new Point(270, y) });
            cmbRoomStatus = new ComboBox { Location = new Point(330, y), DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(150, 20) };
            cmbRoomStatus.Items.AddRange(Enum.GetNames(typeof(RoomStatus)));
            formPanel.Controls.Add(cmbRoomStatus);

            y += 30;
            formPanel.Controls.Add(new Label { Text = "Tipe Kamar:", Location = new Point(10, y) });
            cmbRoomType = new ComboBox { Location = new Point(100, y), DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(150, 20) };
            cmbRoomType.DisplayMember = "TypeName";
            cmbRoomType.ValueMember = "RoomTypeId";
            formPanel.Controls.Add(cmbRoomType);

            y += 30;
            formPanel.Controls.Add(new Label { Text = "Harga:", Location = new Point(10, y) });
            numRoomPrice = new NumericUpDown { Location = new Point(100, y), Size = new Size(150, 20), Maximum = 10000000, Increment = 50000, ThousandsSeparator = true, Enabled = false };
            formPanel.Controls.Add(numRoomPrice);

            y += 30;
            formPanel.Controls.Add(new Label { Text = "Fasilitas:", Location = new Point(10, y) });
            txtRoomFacilities = new TextBox { Location = new Point(100, y), Size = new Size(380, 50), Multiline = true, ScrollBars = ScrollBars.Vertical, Enabled = false };
            formPanel.Controls.Add(txtRoomFacilities);

            // Buttons
            y += 70;
            btnAddRoom = new Button { Text = "Tambah", Location = new Point(10, y), Size = new Size(100, 30) };
            btnUpdateRoom = new Button { Text = "Update", Location = new Point(120, y), Size = new Size(100, 30) };
            btnDeleteRoom = new Button { Text = "Hapus", Location = new Point(230, y), Size = new Size(100, 30), BackColor = Color.Salmon };

            btnAddRoom.Click += BtnAddRoom_Click;
            btnUpdateRoom.Click += BtnUpdateRoom_Click;
            btnDeleteRoom.Click += BtnDeleteRoom_Click;

            formPanel.Controls.Add(btnAddRoom);
            formPanel.Controls.Add(btnUpdateRoom);
            formPanel.Controls.Add(btnDeleteRoom);
        }

        private void LoadRoomData()
        {
            var roomData = from room in DataService.Rooms
                           join type in DataService.RoomTypes on room.RoomTypeId equals type.RoomTypeId
                           select new
                           {
                               room.RoomId,
                               room.RoomNumber,
                               type.TypeName,
                               type.Price,
                               room.Status,
                               type.Facilities
                           };

            dgvRooms.DataSource = roomData.ToList();

            cmbRoomType.DataSource = DataService.RoomTypes.ToList();
        }

        private void DgvRooms_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count > 0)
            {
                var selectedRow = dgvRooms.SelectedRows[0];
                int roomId = (int)selectedRow.Cells[0].OwningRow.DataBoundItem.GetType().GetProperty("RoomId").GetValue(selectedRow.Cells[0].OwningRow.DataBoundItem, null);

                selectedRoom = DataService.Rooms.FirstOrDefault(r => r.RoomId == roomId);
                if (selectedRoom != null)
                {
                    txtRoomNumber.Text = selectedRoom.RoomNumber;
                    cmbRoomStatus.SelectedItem = selectedRoom.Status.ToString();
                    cmbRoomType.SelectedValue = selectedRoom.RoomTypeId;

                    var roomType = DataService.RoomTypes.FirstOrDefault(rt => rt.RoomTypeId == selectedRoom.RoomTypeId);
                    if (roomType != null)
                    {
                        numRoomPrice.Value = roomType.Price;
                        txtRoomFacilities.Text = roomType.Facilities;
                    }
                }
            }
        }

        private void BtnAddRoom_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text) || cmbRoomType.SelectedIndex == -1)
            {
                MessageBox.Show("Data belum lengkap."); return;
            }

            var newRoom = new Room
            {
                RoomId = (DataService.Rooms.Any() ? DataService.Rooms.Max(r => r.RoomId) : 0) + 1,
                RoomNumber = txtRoomNumber.Text,
                RoomTypeId = (int)cmbRoomType.SelectedValue,
                Status = (RoomStatus)Enum.Parse(typeof(RoomStatus), cmbRoomStatus.SelectedItem.ToString())
            };
            DataService.Rooms.Add(newRoom);
            LoadRoomData();
            MessageBox.Show("Kamar baru berhasil ditambahkan.");
        }

        private void BtnUpdateRoom_Click(object sender, EventArgs e)
        {
            if (selectedRoom == null)
            {
                MessageBox.Show("Pilih kamar yang akan diupdate."); return;
            }

            selectedRoom.RoomNumber = txtRoomNumber.Text;
            selectedRoom.RoomTypeId = (int)cmbRoomType.SelectedValue;
            selectedRoom.Status = (RoomStatus)Enum.Parse(typeof(RoomStatus), cmbRoomStatus.SelectedItem.ToString());

            LoadRoomData();
            MessageBox.Show("Data kamar berhasil diupdate.");
        }

        private void BtnDeleteRoom_Click(object sender, EventArgs e)
        {
            if (selectedRoom == null) { MessageBox.Show("Pilih kamar yang akan dihapus."); return; }
            if (DataService.Tenants.Any(t => t.RoomId == selectedRoom.RoomId && t.CheckOutDate == null)) { MessageBox.Show("Kamar tidak bisa dihapus karena masih ada penghuni aktif.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            var confirm = MessageBox.Show("Anda yakin ingin menghapus kamar ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                DataService.Rooms.Remove(selectedRoom);
                selectedRoom = null;
                LoadRoomData();
                MessageBox.Show("Kamar berhasil dihapus.");
            }
        }
        #endregion

        #region Kelola Penghuni Tab
        private void InitializeTenantsTab(TabPage page)
        {
            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 400, IsSplitterFixed = true };
            page.Controls.Add(splitContainer);

            dgvTenants = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false };
            dgvTenants.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nama Penghuni", DataPropertyName = "FullName", Width = 150 });
            dgvTenants.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "No. HP", DataPropertyName = "PhoneNumber", Width = 120 });
            dgvTenants.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "No. Kamar", DataPropertyName = "RoomNumber", Width = 100 });
            dgvTenants.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipe Kamar", DataPropertyName = "RoomType", Width = 120 });
            dgvTenants.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tanggal Masuk", DataPropertyName = "CheckInDate", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd MMMM yyyy" }, Width = 150 });
            dgvTenants.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tanggal Keluar", DataPropertyName = "CheckOutDate", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd MMMM yyyy" }, Width = 150 });

            splitContainer.Panel1.Controls.Add(dgvTenants);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            btnRemoveTenant = new Button { Text = "Hapus/Keluarkan Penghuni", Location = new Point(10, 10), Size = new Size(200, 30), BackColor = Color.LightCoral };
            btnRemoveTenant.Click += BtnRemoveTenant_Click;
            panel.Controls.Add(btnRemoveTenant);
            splitContainer.Panel2.Controls.Add(panel);
        }

        private void LoadTenantData()
        {
            var tenantData = from tenant in DataService.Tenants
                             join user in DataService.Users on tenant.UserId equals user.UserId
                             join room in DataService.Rooms on tenant.RoomId equals room.RoomId
                             join roomType in DataService.RoomTypes on room.RoomTypeId equals roomType.RoomTypeId
                             select new
                             {
                                 tenant.TenantId,
                                 user.FullName,
                                 user.PhoneNumber,
                                 room.RoomNumber,
                                 RoomType = roomType.TypeName,
                                 tenant.CheckInDate,
                                 tenant.CheckOutDate
                             };

            dgvTenants.DataSource = tenantData.Where(t => t.CheckOutDate == null).ToList();
        }

        private void BtnRemoveTenant_Click(object sender, EventArgs e)
        {
            if (dgvTenants.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih penghuni yang akan dikeluarkan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvTenants.SelectedRows[0];
            int tenantId = (int)selectedRow.DataBoundItem.GetType().GetProperty("TenantId").GetValue(selectedRow.DataBoundItem, null);

            var tenant = DataService.Tenants.FirstOrDefault(t => t.TenantId == tenantId);
            if (tenant != null)
            {
                var confirm = MessageBox.Show($"Anda yakin ingin mengeluarkan penghuni '{selectedRow.Cells[0].Value}' dari kamar '{selectedRow.Cells[2].Value}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    tenant.CheckOutDate = DateTime.Now;
                    var room = DataService.Rooms.FirstOrDefault(r => r.RoomId == tenant.RoomId);
                    if (room != null)
                    {
                        room.Status = RoomStatus.Tersedia;
                    }
                    LoadTenantData();
                    LoadRoomData();
                    LoadDashboardData();
                    MessageBox.Show("Penghuni berhasil dikeluarkan dan status kamar telah diupdate menjadi tersedia.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion

        #region Kelola Tagihan Tab
        private void InitializeBillingsTab(TabPage page)
        {
            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 400, IsSplitterFixed = true };
            page.Controls.Add(splitContainer);

            dgvBillings = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false };
            dgvBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nama Penghuni", DataPropertyName = "FullName", Width = 150 });
            dgvBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "No. Kamar", DataPropertyName = "RoomNumber", Width = 80 });
            dgvBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Jumlah Tagihan", DataPropertyName = "Amount", DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" }, Width = 120 });
            dgvBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Jatuh Tempo", DataPropertyName = "DueDate", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd MMMM yyyy" }, Width = 120 });
            dgvBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 120 });
            dgvBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Bukti Bayar", DataPropertyName = "ProofOfPaymentPath", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            splitContainer.Panel1.Controls.Add(dgvBillings);

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            btnCreateBilling = new Button { Text = "Buat Tagihan Baru", Size = new Size(150, 30) };
            btnVerifyPayment = new Button { Text = "Verifikasi Pembayaran", Size = new Size(150, 30), BackColor = Color.LightGreen };
            btnSetUnpaid = new Button { Text = "Tandai Belum Lunas", Size = new Size(150, 30), BackColor = Color.LightSalmon };

            btnCreateBilling.Click += (s, e) => MessageBox.Show("Fitur pembuatan tagihan otomatis akan dikembangkan selanjutnya. Saat ini tagihan dibuat saat penghuni memesan kamar.");
            btnVerifyPayment.Click += BtnVerifyPayment_Click;
            btnSetUnpaid.Click += BtnSetUnpaid_Click;

            panel.Controls.Add(btnCreateBilling);
            panel.Controls.Add(btnVerifyPayment);
            panel.Controls.Add(btnSetUnpaid);
            splitContainer.Panel2.Controls.Add(panel);
        }

        private void LoadBillingData()
        {
            var billingData = from billing in DataService.Billings
                              join tenant in DataService.Tenants on billing.TenantId equals tenant.TenantId
                              join user in DataService.Users on tenant.UserId equals user.UserId
                              join room in DataService.Rooms on tenant.RoomId equals room.RoomId
                              orderby billing.DueDate
                              select new
                              {
                                  billing.BillingId,
                                  user.FullName,
                                  room.RoomNumber,
                                  billing.Amount,
                                  billing.DueDate,
                                  billing.Status,
                                  billing.ProofOfPaymentPath
                              };

            dgvBillings.DataSource = billingData.ToList();
        }

        private void BtnVerifyPayment_Click(object sender, EventArgs e)
        {
            if (dgvBillings.SelectedRows.Count == 0) { MessageBox.Show("Pilih tagihan yang akan diverifikasi."); return; }

            var selectedRow = dgvBillings.SelectedRows[0];
            int billingId = (int)selectedRow.DataBoundItem.GetType().GetProperty("BillingId").GetValue(selectedRow.DataBoundItem, null);

            var billing = DataService.Billings.FirstOrDefault(b => b.BillingId == billingId);
            if (billing != null && billing.Status == BillingStatus.MenungguVerifikasi)
            {
                var confirm = MessageBox.Show($"Verifikasi pembayaran untuk '{selectedRow.Cells[0].Value}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    billing.Status = BillingStatus.Lunas;
                    LoadBillingData();
                    MessageBox.Show("Pembayaran berhasil diverifikasi.");
                }
            }
            else
            {
                MessageBox.Show("Hanya tagihan dengan status 'Menunggu Verifikasi' yang bisa diverifikasi.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSetUnpaid_Click(object sender, EventArgs e)
        {
            if (dgvBillings.SelectedRows.Count == 0) { MessageBox.Show("Pilih tagihan yang akan ditandai."); return; }

            var selectedRow = dgvBillings.SelectedRows[0];
            int billingId = (int)selectedRow.DataBoundItem.GetType().GetProperty("BillingId").GetValue(selectedRow.DataBoundItem, null);

            var billing = DataService.Billings.FirstOrDefault(b => b.BillingId == billingId);
            if (billing != null)
            {
                var confirm = MessageBox.Show($"Ubah status tagihan untuk '{selectedRow.Cells[0].Value}' menjadi 'Belum Lunas'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    billing.Status = BillingStatus.BelumLunas;
                    billing.ProofOfPaymentPath = null; // Hapus path bukti bayar
                    LoadBillingData();
                    MessageBox.Show("Status tagihan berhasil diubah.");
                }
            }
        }
        #endregion
    }
}