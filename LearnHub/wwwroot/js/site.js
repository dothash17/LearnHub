function showLogin() {
    document.getElementById("loginForm").style.display = "block";
    document.getElementById("registerForm").style.display = "none";
    document.querySelector('#loginModal .full-width-btn.active').classList.remove('active');
    document.querySelector('#loginModal .full-width-btn:nth-child(1)').classList.add('active');
}

function showRegister() {
    document.getElementById("loginForm").style.display = "none";
    document.getElementById("registerForm").style.display = "block";
    document.querySelector('#loginModal .full-width-btn.active').classList.remove('active');
    document.querySelector('#loginModal .full-width-btn:nth-child(2)').classList.add('active');
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

document.addEventListener("DOMContentLoaded", function () {
    const slides = document.querySelectorAll('.slide');
    const pagination = document.querySelector('.pagination');

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
});

document.addEventListener("DOMContentLoaded", function () {
    function loadLessonContent(lessonId) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/Lesson/GetLessonText?id=' + lessonId, true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                document.getElementById('lesson-text').innerHTML = xhr.responseText;
            } else {
                document.getElementById('lesson-text').innerHTML = 'Ошибка при загрузке содержимого урока.';
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
        }
    });

    document.getElementById('next-lesson-btn').addEventListener('click', function () {
        if (currentLessonIndex < lessonItems.length - 1) {
            currentLessonIndex++;
            updateActiveLesson();
            loadCurrentLesson();
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
            'link', 'uploadImage', 'blockQuote', 'insertTable', 'codeBlock', '|',
            'specialCharacters', 'horizontalLine', 'pageBreak', '|',
        ],
        shouldNotGroupWhenFull: true
    },

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

function showReviewForm() {
    document.getElementById("reviewForm").style.display = "block";
    document.getElementById("showReviewButton").style.display = "none";
}

function hideReviewForm() {
    document.getElementById("reviewForm").style.display = "none";
    document.getElementById("showReviewButton").style.display = "block";
}