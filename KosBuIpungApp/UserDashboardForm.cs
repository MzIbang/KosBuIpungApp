// ===== Views/UserDashboardForm.cs =====
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KosBuIpungApp.Enums;
using KosBuIpungApp.Models;
using KosBuIpungApp.Services;

namespace KosBuIpungApp.Views
{
    public class UserDashboardForm : Form
    {
        private TabControl mainTabControl;

        // Controls untuk Pesan Kamar
        private DataGridView dgvAvailableRooms;
        private Button btnBookRoom;

        // Controls untuk Tagihan
        private DataGridView dgvMyBillings;
        private Button btnUploadProof;

        // Controls untuk Profil
        private TextBox txtFullName, txtPhoneNumber, txtUsername;
        private Button btnUpdateProfile;

        public UserDashboardForm()
        {
            InitializeComponent();
            LoadAvailableRooms();
            LoadMyBillings();
            LoadMyProfile();
        }

        private void InitializeComponent()
        {
            this.Text = $"User Dashboard - Selamat Datang, {AuthService.CurrentUser.FullName}";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosed += (s, e) => Application.Exit();

            mainTabControl = new TabControl { Dock = DockStyle.Fill };
            this.Controls.Add(mainTabControl);

            var tabPageBooking = new TabPage("Pesan Kamar");
            var tabPageBillings = new TabPage("Tagihan Saya");
            var tabPageProfile = new TabPage("Profil Saya");

            mainTabControl.TabPages.Add(tabPageBooking);
            mainTabControl.TabPages.Add(tabPageBillings);
            mainTabControl.TabPages.Add(tabPageProfile);

            InitializeBookingTab(tabPageBooking);
            InitializeBillingsTab(tabPageBillings);
            InitializeProfileTab(tabPageProfile);
        }

        #region Pesan Kamar Tab
        private void InitializeBookingTab(TabPage page)
        {
            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300, IsSplitterFixed = true };
            page.Controls.Add(splitContainer);

            dgvAvailableRooms = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false };
            dgvAvailableRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "No. Kamar", DataPropertyName = "RoomNumber", Width = 100 });
            dgvAvailableRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipe Kamar", DataPropertyName = "TypeName", Width = 150 });
            dgvAvailableRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Harga / Bulan", DataPropertyName = "Price", DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" }, Width = 120 });
            dgvAvailableRooms.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fasilitas", DataPropertyName = "Facilities", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            splitContainer.Panel1.Controls.Add(dgvAvailableRooms);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            btnBookRoom = new Button { Text = "Pesan Kamar Terpilih", Location = new Point(10, 10), Size = new Size(200, 40), Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.SeaGreen, ForeColor = Color.White };
            btnBookRoom.Click += BtnBookRoom_Click;
            panel.Controls.Add(btnBookRoom);
            splitContainer.Panel2.Controls.Add(panel);
        }

        private void LoadAvailableRooms()
        {
            var roomData = from room in DataService.Rooms
                           where room.Status == RoomStatus.Tersedia
                           join type in DataService.RoomTypes on room.RoomTypeId equals type.RoomTypeId
                           select new
                           {
                               room.RoomId,
                               room.RoomNumber,
                               type.TypeName,
                               type.Price,
                               type.Facilities
                           };

            dgvAvailableRooms.DataSource = roomData.ToList();
        }

        private void BtnBookRoom_Click(object sender, EventArgs e)
        {
            if (DataService.Tenants.Any(t => t.UserId == AuthService.CurrentUser.UserId && t.CheckOutDate == null))
            {
                MessageBox.Show("Anda sudah terdaftar sebagai penghuni aktif. Tidak bisa memesan kamar lagi.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvAvailableRooms.SelectedRows.Count == 0)
            {
                MessageBox.Show("Silakan pilih kamar yang ingin dipesan.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvAvailableRooms.SelectedRows[0];
            int roomId = (int)selectedRow.DataBoundItem.GetType().GetProperty("RoomId").GetValue(selectedRow.DataBoundItem, null);
            decimal price = (decimal)selectedRow.DataBoundItem.GetType().GetProperty("Price").GetValue(selectedRow.DataBoundItem, null);

            var confirm = MessageBox.Show($"Anda yakin ingin memesan kamar {selectedRow.Cells[0].Value}?", "Konfirmasi Pemesanan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                // 1. Buat record Tenant baru
                var newTenant = new Tenant
                {
                    TenantId = (DataService.Tenants.Any() ? DataService.Tenants.Max(t => t.TenantId) : 0) + 1,
                    UserId = AuthService.CurrentUser.UserId,
                    RoomId = roomId,
                    CheckInDate = DateTime.Now
                };
                DataService.Tenants.Add(newTenant);

                // 2. Update status kamar
                var room = DataService.Rooms.First(r => r.RoomId == roomId);
                room.Status = RoomStatus.Terisi;

                // 3. Buat tagihan pertama
                var newBilling = new Billing
                {
                    BillingId = (DataService.Billings.Any() ? DataService.Billings.Max(b => b.BillingId) : 0) + 1,
                    TenantId = newTenant.TenantId,
                    Amount = price,
                    DueDate = DateTime.Now.AddDays(7), // Jatuh tempo 7 hari dari sekarang
                    Status = BillingStatus.BelumLunas
                };
                DataService.Billings.Add(newBilling);

                MessageBox.Show("Pemesanan berhasil! Silakan cek tab 'Tagihan Saya' untuk melakukan pembayaran.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAvailableRooms();
                LoadMyBillings();
            }
        }
        #endregion

        #region Tagihan Saya Tab
        private void InitializeBillingsTab(TabPage page)
        {
            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300, IsSplitterFixed = true };
            page.Controls.Add(splitContainer);

            dgvMyBillings = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false };
            dgvMyBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Jumlah Tagihan", DataPropertyName = "Amount", DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" }, Width = 150 });
            dgvMyBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Jatuh Tempo", DataPropertyName = "DueDate", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd MMMM yyyy" }, Width = 150 });
            dgvMyBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 150 });
            dgvMyBillings.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Bukti Bayar", DataPropertyName = "ProofOfPaymentPath", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            splitContainer.Panel1.Controls.Add(dgvMyBillings);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            btnUploadProof = new Button { Text = "Upload Bukti Pembayaran", Location = new Point(10, 10), Size = new Size(200, 40), Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.DodgerBlue, ForeColor = Color.White };
            btnUploadProof.Click += BtnUploadProof_Click;
            panel.Controls.Add(btnUploadProof);
            splitContainer.Panel2.Controls.Add(panel);
        }

        private void LoadMyBillings()
        {
            var myTenant = DataService.Tenants.FirstOrDefault(t => t.UserId == AuthService.CurrentUser.UserId && t.CheckOutDate == null);
            if (myTenant != null)
            {
                var billingData = DataService.Billings
                    .Where(b => b.TenantId == myTenant.TenantId)
                    .OrderByDescending(b => b.DueDate)
                    .ToList();
                dgvMyBillings.DataSource = billingData;
            }
            else
            {
                dgvMyBillings.DataSource = null;
            }
        }

        private void BtnUploadProof_Click(object sender, EventArgs e)
        {
            if (dgvMyBillings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih tagihan yang akan dibayar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvMyBillings.SelectedRows[0];
            var billing = (Billing)selectedRow.DataBoundItem;

            if (billing.Status == BillingStatus.Lunas || billing.Status == BillingStatus.MenungguVerifikasi)
            {
                MessageBox.Show("Tagihan ini sudah lunas atau sedang menunggu verifikasi.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
                ofd.Title = "Pilih Bukti Pembayaran";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    billing.ProofOfPaymentPath = ofd.FileName;
                    billing.Status = BillingStatus.MenungguVerifikasi;
                    LoadMyBillings();
                    MessageBox.Show("Bukti pembayaran berhasil diunggah. Mohon tunggu verifikasi dari admin.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        #region Profil Saya Tab
        private void InitializeProfileTab(TabPage page)
        {
            page.Padding = new Padding(20);

            var panel = new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(panel);

            int y = 20;
            panel.Controls.Add(new Label { Text = "Username:", Location = new Point(20, y), Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            txtUsername = new TextBox { Location = new Point(150, y), Size = new Size(250, 25), Font = new Font("Segoe UI", 10), ReadOnly = true, BackColor = Color.Gainsboro };
            panel.Controls.Add(txtUsername);

            y += 40;
            panel.Controls.Add(new Label { Text = "Nama Lengkap:", Location = new Point(20, y), Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            txtFullName = new TextBox { Location = new Point(150, y), Size = new Size(250, 25), Font = new Font("Segoe UI", 10) };
            panel.Controls.Add(txtFullName);

            y += 40;
            panel.Controls.Add(new Label { Text = "Nomor Telepon:", Location = new Point(20, y), Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            txtPhoneNumber = new TextBox { Location = new Point(150, y), Size = new Size(250, 25), Font = new Font("Segoe UI", 10) };
            panel.Controls.Add(txtPhoneNumber);

            y += 60;
            btnUpdateProfile = new Button { Text = "Update Profil", Location = new Point(150, y), Size = new Size(150, 40), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnUpdateProfile.Click += BtnUpdateProfile_Click;
            panel.Controls.Add(btnUpdateProfile);
        }

        private void LoadMyProfile()
        {
            var user = AuthService.CurrentUser;
            txtUsername.Text = user.Username;
            txtFullName.Text = user.FullName;
            txtPhoneNumber.Text = user.PhoneNumber;
        }

        private void BtnUpdateProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Nama dan Nomor Telepon tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = DataService.Users.First(u => u.UserId == AuthService.CurrentUser.UserId);
            user.FullName = txtFullName.Text;
            user.PhoneNumber = txtPhoneNumber.Text;

            // Perbarui juga data di AuthService jika perlu, meskipun CurrentUser adalah reference type
            AuthService.CurrentUser.FullName = txtFullName.Text;
            AuthService.CurrentUser.PhoneNumber = txtPhoneNumber.Text;

            MessageBox.Show("Profil berhasil diperbarui.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Text = $"User Dashboard - Selamat Datang, {AuthService.CurrentUser.FullName}";
        }
        #endregion
    }
}