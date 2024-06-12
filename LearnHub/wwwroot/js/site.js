function showLogin() {
    document.getElementById("loginForm").style.display = "block";
    document.getElementById("registerForm").style.display = "none";
    document.getElementById("forgotPasswordForm").style.display = "none";
    document.getElementById("authButtons").style.display = "flex";
    document.querySelector('#loginModal .full-width-btn.active').classList.remove('active');
    document.querySelector('#loginModal .full-width-btn:nth-child(1)').classList.add('active');
}

function showRegister() {
    document.getElementById("loginForm").style.display = "none";
    document.getElementById("registerForm").style.display = "block";
    document.getElementById("forgotPasswordForm").style.display = "none";
    document.getElementById("authButtons").style.display = "flex";
    document.querySelector('#loginModal .full-width-btn.active').classList.remove('active');
    document.querySelector('#loginModal .full-width-btn:nth-child(2)').classList.add('active');
}

function showForgotPassword() {
    document.getElementById("loginForm").style.display = "none";
    document.getElementById("registerForm").style.display = "none";
    document.getElementById("forgotPasswordForm").style.display = "block";
    document.getElementById("authButtons").style.display = "none";
}

function validateLoginForm() {
    var formElement = document.getElementById("loginForm");
    var formData = new FormData(formElement);

    fetch(formElement.action, {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (response.ok) {
            window.location.href = '/Home/Index';
        } else {
            return response.text();
        }
    })
    .then(errorText => {
        if (errorText) {
            document.getElementById("loginError").innerText = errorText;
            $('#loginModal').modal('show');
        }
    });
}

function validateRegisterForm() {
    var formElement = document.getElementById("registerForm");
    var formData = new FormData(formElement);

    if (!formElement.checkValidity()) {
        formElement.reportValidity();
        return;
    }

    fetch(formElement.action, {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (response.ok) {
            window.location.href = '/Home/Index';
        } else {
            return response.text();
        }
    })
    .then(errorText => {
        if (errorText) {
            document.getElementById("registerError").innerText = errorText;
            $('#loginModal').modal('show');
        }
    });
}

function scrollToBottom() {
    var chatMessages = document.getElementById("chatMessages");
    if (chatMessages) {
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }
}

window.onload = function () {
    scrollToBottom();
};

function showReviewForm() {
    document.getElementById("reviewForm").style.display = "block";
    document.getElementById("showReviewButton").style.display = "none";
}

function hideReviewForm() {
    document.getElementById("reviewForm").style.display = "none";
    document.getElementById("showReviewButton").style.display = "block";
}

document.querySelectorAll('.rating-stars input').forEach((star) => {
    star.addEventListener('change', function () {
        let stars = document.querySelectorAll('.rating-stars .star');
        let checkedValue = this.value;
        stars.forEach((label, index) => {
            if (index < checkedValue) {
                label.classList.add('checked');
            } else {
                label.classList.remove('checked');
            }
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    var lessonContent;

    function loadLessonContent(lessonId) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/Lesson/GetLessonContent?id=' + lessonId, true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                lessonContent = JSON.parse(xhr.responseText);
                document.getElementById('lesson-text').innerHTML = lessonContent.text;
                var assignmentsContainer = document.getElementById('lesson-assignments');
                assignmentsContainer.innerHTML = '';
                lessonContent.assignments.forEach(function (assignment, index) {
                    var assignmentDiv = document.createElement('div');
                    assignmentDiv.innerHTML = assignment.task;
                    assignmentsContainer.appendChild(assignmentDiv);

                    var input = document.createElement('input');
                    input.type = 'text';
                    input.id = 'answer-input-' + index;
                    input.placeholder = 'Введите ваш ответ';
                    input.className = 'input-field';
                    input.autocomplete = "off";
                    assignmentsContainer.appendChild(input);

                    var button = document.createElement('button');
                    button.textContent = 'Проверить ответ';
                    button.className = 'check-button';
                    button.onclick = function () {
                        checkAnswer(index, assignment.id);
                    };
                    assignmentsContainer.appendChild(button);
                    if (assignment.solved) {
                        input.value = assignment.answer;
                        input.disabled = true;
                        button.disabled = true;
                        button.className += ' disabled';
                    }
                });
            } else {
                document.getElementById('lesson-text').innerHTML = 'Ошибка при загрузке содержимого урока.';
            }
        };
        xhr.send();
    }

    function showNotification(message, isError) {
        var notification = document.getElementById('notification');
        notification.textContent = message;
        notification.className = 'notification' + (isError ? ' error' : '');
        notification.style.display = 'block';
        setTimeout(function () {
            notification.style.display = 'none';
        }, 3000);
    }

    function checkAnswer(assignmentIndex, assignmentId) {
        var userAnswer = document.getElementById('answer-input-' + assignmentIndex).value.trim().toLowerCase();
        var correctAnswer = lessonContent.assignments[assignmentIndex].answer.toLowerCase();

        if (userAnswer === correctAnswer || correctAnswer.includes(userAnswer)) {
            showNotification('Правильный ответ!', false);
            recordProgress(assignmentId);
            var input = document.getElementById('answer-input-' + assignmentIndex);
            var button = input.nextElementSibling;
            input.disabled = true;
            button.disabled = true;
            button.className += ' disabled';
        } else {
            showNotification('Неправильный ответ. Попробуйте еще раз.', true);
        }
    }

    function recordProgress(assignmentId) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Lesson/RecordProgress?assignmentId=' + assignmentId, true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                console.log('Прогресс успешно записан');
            } else {
                console.error('Ошибка при записи прогресса');
            }
        };
        xhr.send();
    }

    var lessonItems = document.querySelectorAll('.lesson-list li');
    var currentLessonIndex = 0;

    function updateActiveLesson() {
        document.querySelectorAll('.lesson-list li').forEach(function (li) {
            li.classList.remove('active');
        });
        lessonItems[currentLessonIndex].classList.add('active');
    }

    function loadCurrentLesson() {
        var lessonId = lessonItems[currentLessonIndex].dataset.lessonId;
        loadLessonContent(lessonId);
        window.scrollTo(0, 0);
    }

    updateActiveLesson();
    loadCurrentLesson();

    lessonItems.forEach(function (item, index) {
        item.addEventListener('click', function () {
            currentLessonIndex = index;
            updateActiveLesson();
            loadCurrentLesson();
        });
    });

    document.getElementById('prev-lesson-btn').addEventListener('click', function () {
        if (currentLessonIndex > 0) {
            currentLessonIndex--;
            updateActiveLesson();
            loadCurrentLesson();
            window.scrollTo(0, 0);
        }
    });

    document.getElementById('next-lesson-btn').addEventListener('click', function () {
        if (currentLessonIndex < lessonItems.length - 1) {
            currentLessonIndex++;
            updateActiveLesson();
            loadCurrentLesson();
            window.scrollTo(0, 0);
        }
    });
});

CKEDITOR.ClassicEditor.create(document.getElementById("editor"), {
    toolbar: {
        items: [
            'exportPDF', 'exportWord', '|',
            'heading', 'fontSize', 'alignment', '|',
            'bold', 'italic', 'underline', 'code', 'subscript', 'superscript', 'removeFormat', '|',
            'bulletedList', 'numberedList', 'todoList', '|',
            'outdent', 'indent', '|',
            'undo', 'redo',
            '-',
            'link', 'uploadImage', 'mediaEmbed', 'blockQuote', 'insertTable', 'codeBlock', '|',
            'specialCharacters', 'horizontalLine', 'pageBreak', '|',
        ],
        shouldNotGroupWhenFull: true
    },

    mediaEmbed: { previewsInData: true },

    list: {
        properties: {
            styles: true,
            startIndex: true,
            reversed: true
        }
    },

    heading: {
        options: [
            { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
            { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
            { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' },
            { model: 'heading3', view: 'h3', title: 'Heading 3', class: 'ck-heading_heading3' },
            { model: 'heading4', view: 'h4', title: 'Heading 4', class: 'ck-heading_heading4' },
            { model: 'heading5', view: 'h5', title: 'Heading 5', class: 'ck-heading_heading5' },
            { model: 'heading6', view: 'h6', title: 'Heading 6', class: 'ck-heading_heading6' }
        ]
    },

    fontSize: {
        options: [10, 12, 14, 'default', 18, 20, 22],
        supportAllValues: true
    },

    link: {
        decorators: {
            addTargetToExternalLinks: true,
            defaultProtocol: 'https://',
            toggleDownloadable: {
                mode: 'manual',
                label: 'Downloadable',
                attributes: {
                    download: 'file'
                }
            }
        }
    },

    mention: {
        feeds: [
            {
                marker: '@',
                feed: [
                    '@apple', '@bears', '@brownie', '@cake', '@cake', '@candy', '@canes', '@chocolate', '@cookie', '@cotton', '@cream',
                    '@cupcake', '@danish', '@donut', '@dragée', '@fruitcake', '@gingerbread', '@gummi', '@ice', '@jelly-o',
                    '@liquorice', '@macaroon', '@marzipan', '@oat', '@pie', '@plum', '@pudding', '@sesame', '@snaps', '@soufflé',
                    '@sugar', '@sweet', '@topping', '@wafer'
                ],
                minimumCharacters: 1
            }
        ]
    },

    removePlugins: [
        'AIAssistant',
        'CKBox',
        'CKFinder',
        'EasyImage',
        'MultiLevelList',
        'RealTimeCollaborativeComments',
        'RealTimeCollaborativeTrackChanges',
        'RealTimeCollaborativeRevisionHistory',
        'PresenceList',
        'Comments',
        'TrackChanges',
        'TrackChangesData',
        'RevisionHistory',
        'Pagination',
        'WProofreader',
        'MathType',
        'SlashCommand',
        'Template',
        'DocumentOutline',
        'FormatPainter',
        'TableOfContents',
        'PasteFromOfficeEnhanced',
        'CaseChange'
    ]
});

document.addEventListener("DOMContentLoaded", function () {
    const slides = document.querySelectorAll('.slide');
    const pagination = document.querySelector('.pagination');
    const sortButtons = document.querySelectorAll('.sort-buttons .sort-button');
    const sortButtonsList = document.querySelectorAll('.sort-buttons-list .sort-button');

    sortButtons.forEach(button => {
        button.addEventListener('click', () => {
            sortButtons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');

            const sortType = button.id.replace('sort-sl-', '');
            sortSlides(`data-${sortType}`);
        });
    });

    sortButtonsList.forEach(button => {
        button.addEventListener('click', () => {
            sortButtonsList.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');

            const sortType = button.id.replace('sort-list-', '');
            sortCoursesList(`data-${sortType}`);
        });
    });

    slides.forEach((slide, index) => {
        const dot = document.createElement('span');
        dot.addEventListener('click', () => {
            goToSlide(index);
        });
        pagination.appendChild(dot);
    });

    let currentSlide = 0;
    updateSlider();

    function updateSlider() {
        slides.forEach((slide, index) => {
            slide.style.display = index === currentSlide ? 'flex' : 'none';
        });

        const dots = pagination.querySelectorAll('span');
        dots.forEach((dot, index) => {
            if (index === currentSlide) {
                dot.classList.add('active');
            } else {
                dot.classList.remove('active');
            }
        });

        const prevBtn = document.querySelector('.prev');
        const nextBtn = document.querySelector('.next');
        if (currentSlide === 0) {
            prevBtn.style.display = 'none';
        } else {
            prevBtn.style.display = 'block';
        }
        if (currentSlide === 2) {
            nextBtn.style.display = 'none';
        } else {
            nextBtn.style.display = 'block';
        }
    }

    function goToSlide(index) {
        if (index < 0 || index >= slides.length) return;
        currentSlide = index;
        updateSlider();
    }

    const prevBtn = document.querySelector('.prev');
    const nextBtn = document.querySelector('.next');

    prevBtn.addEventListener('click', () => {
        goToSlide(currentSlide - 1);
    });

    nextBtn.addEventListener('click', () => {
        goToSlide(currentSlide + 1);
    });

    document.getElementById('sort-sl-enrollments').addEventListener('click', () => {
        sortSlides('data-enrollments');
    });

    document.getElementById('sort-sl-rating').addEventListener('click', () => {
        sortSlides('data-rating');
    });

    document.getElementById('sort-sl-date').addEventListener('click', () => {
        sortSlides('data-date');
    });

    document.getElementById('sort-list-enrollments').addEventListener('click', () => {
        sortCoursesList('data-enrollments');
    });

    document.getElementById('sort-list-rating').addEventListener('click', () => {
        sortCoursesList('data-rating');
    });

    document.getElementById('sort-list-date').addEventListener('click', () => {
        sortCoursesList('data-date');
    });

    function sortSlides(dataAttribute) {
        const slideContainer = document.getElementById('slide-container');
        const slides = Array.from(slideContainer.querySelectorAll('.slide'));

        const tempContainer = document.createElement('div');
        let courses = Array.from(slideContainer.querySelectorAll('.catalog-card'));

        courses.sort((a, b) => {
            const aValue = a.getAttribute(dataAttribute);
            const bValue = b.getAttribute(dataAttribute);

            if (dataAttribute === 'data-rating' || dataAttribute === 'data-enrollments') {
                const aNumericValue = parseFloat(aValue) || 0;
                const bNumericValue = parseFloat(bValue) || 0;
                return bNumericValue - aNumericValue;
            } else {
                const aDate = new Date(aValue);
                const bDate = new Date(bValue);
                return bDate - aDate;
            }
        });

        courses.forEach(course => {
            tempContainer.appendChild(course);
        });

        slideContainer.innerHTML = '';

        const slideSize = 4;
        slides.slice(0, 3).forEach(slide => {
            const slideCourses = Array.from(tempContainer.querySelectorAll('.catalog-card'));
            slideCourses.splice(slideSize).forEach(course => {
                tempContainer.appendChild(course);
            });
            slideCourses.forEach(course => {
                slide.appendChild(course);
            });
            slideContainer.appendChild(slide);
        });

        currentSlide = 0;
        updateSlider();
        updatePagination();
    }

    function sortCoursesList(dataAttribute) {
        const courseList = document.getElementById('course-list');
        const courses = Array.from(courseList.querySelectorAll('.courses-item'));

        courses.sort((a, b) => {
            const aValue = a.getAttribute(dataAttribute);
            const bValue = b.getAttribute(dataAttribute);

            if (dataAttribute === 'data-rating' || dataAttribute === 'data-enrollments') {
                const aNumericValue = parseFloat(aValue) || 0;
                const bNumericValue = parseFloat(bValue) || 0;
                return bNumericValue - aNumericValue;
            } else {
                const aDate = new Date(aValue);
                const bDate = new Date(bValue);
                return bDate - aDate;
            }
        });

        courseList.innerHTML = '';
        courses.forEach(course => {
            courseList.appendChild(course);
        });
    }

    function updatePagination() {
        pagination.innerHTML = '';
        const newSlides = document.querySelectorAll('.slide');

        newSlides.forEach((slide, index) => {
            const dot = document.createElement('span');
            dot.addEventListener('click', () => {
                goToSlide(index);
            });
            pagination.appendChild(dot);
        });
        updateSlider();
    }

    sortSlides('data-enrollments');
    sortCoursesList('data-enrollments');
});

document.getElementById('search-input').addEventListener('input', function () {
    var filter = this.value.toLowerCase();
    var courseItems = document.querySelectorAll('.courses-item');
    courseItems.forEach(function (item) {
        var title = item.getAttribute('data-title');
        var author = item.getAttribute('data-author');
        if (title.includes(filter) || author.includes(filter)) {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
});

document.addEventListener('DOMContentLoaded', (event) => {
    var messageInput = document.getElementById("messageText");

    messageInput.addEventListener('input', function () {
        while (messageInput.value.startsWith(" ") || messageInput.value.startsWith("\n")) {
            messageInput.value = messageInput.value.substring(1);
        }
    });

    messageInput.addEventListener('keypress', function (e) {
        if (messageInput.value.length === 0 && e.key === " ") {
            e.preventDefault();
        }
    });
});
