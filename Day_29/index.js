const users = [
    { userName: "abc", email: "abc@gmail.com", age: 25 },
    { userName: "xyz", email: "xyz@gmail.com", age: 30 },
    { userName: "lmn", email: "lmn@gmail.com.", age: 28 }
];

// Promise
function fetchUsersPromise() {
    return new Promise((resolve, reject) => {
        setTimeout(() => resolve(users), 1000);
    });
}

function getUsersWithPromise() {
    clearOutput();
    fetchUsersPromise()
        .then(data => renderUsers(data))
        .catch(err => console.error("Promise Error:", err));
}

// Callback
function fetchUsersCallback(callback) {
    clearOutput();
    setTimeout(() => {
        callback(null, users);
    }, 1000);
}

function getUsersWithCallback() {
    fetchUsersCallback((err, data) => {
        if (err) {
            console.error("Callback Error:", err);
            return;
        }
        renderUsers(data);
    });
}

// Async/Await
function fetchUsersAsync() {
    return new Promise((resolve) => {
        setTimeout(() => resolve(users), 1000);
    });
}

async function getUsersWithAsyncAwait() {
    clearOutput();
    try {
        const data = await fetchUsersAsync();
        renderUsers(data);
    } catch (error) {
        console.error("Async/Await Error:", error);
    }
}

function clearOutput() {
    document.getElementById('output').innerHTML = '';
}

function renderUsers(data) {
    const output = document.getElementById('output');
    output.innerHTML = ''; 
    data.forEach(user => {
        const div = document.createElement('div');
        div.className = 'user';
        div.innerHTML = `<strong>${user.userName}</strong> - ${user.email} - Age: ${user.age}`;
        output.appendChild(div);
    });
}