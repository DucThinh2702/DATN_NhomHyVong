
    function validateUserForm() {
        let isValid = true;

            // Xóa lỗi cũ
            document.querySelectorAll('.text-danger').forEach(el => el.textContent = '');

    // Lấy các giá trị
    const fullName = document.getElementById('fullName').value.trim();
    const username = document.getElementById('username').value.trim();
    const email = document.getElementById('email').value.trim();
    const phone = document.getElementById('phoneNumber').value.trim();
    const address = document.getElementById('address').value.trim();
    const birthDate = document.getElementById('birthDate').value;
    const gender = document.getElementById('gender').value;
    const roleId = document.getElementById('roleId').value;
    const password = document.getElementById('password').value;
    const isCreate = document.getElementById('submitBtn').textContent.trim() === "Thêm mới";

    // Bắt lỗi từng trường
    if (fullName === '') {
        document.getElementById('fullNameError').textContent = 'Vui lòng nhập họ tên';
    isValid = false;
            }

    if (username === '') {
        document.getElementById('usernameError').textContent = 'Vui lòng nhập tên đăng nhập';
    isValid = false;
            }

    if (email === '') {
        document.getElementById('emailError').textContent = 'Vui lòng nhập email';
    isValid = false;
    } else if (!/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/i.test(email)) {
        document.getElementById('emailError').textContent = 'Email không hợp lệ';
    isValid = false;
            }

    if (isCreate && password === '') {
        document.getElementById('passwordGroup').insertAdjacentHTML('beforeend',
            '<span id="passwordError" class="text-danger">Vui lòng nhập mật khẩu</span>');
    isValid = false;
            }

    if (phone === '') {
        document.getElementById('phoneNumberError').textContent = 'Vui lòng nhập số điện thoại';
    isValid = false;
            } else if (!/^[0-9]{10}$/.test(phone)) {
        document.getElementById('phoneNumberError').textContent = 'Số điện thoại không hợp lệ';
    isValid = false;
            }

    if (address === '') {
        document.getElementById('addressError').textContent = 'Vui lòng nhập địa chỉ';
    isValid = false;
            }

    if (birthDate === '') {
        document.getElementById('birthDateError').textContent = 'Vui lòng chọn ngày sinh';
    isValid = false;
            }

    if (gender === '') {
        document.getElementById('genderError').textContent = 'Vui lòng chọn giới tính';
    isValid = false;
            }

    if (roleId === '') {
        document.getElementById('roleIdError').textContent = 'Vui lòng chọn vai trò';
    isValid = false;
            }

    return isValid;
        }
