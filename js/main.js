const menuButton = document.querySelector('.menu-toggle');
const siteNav = document.querySelector('#site-nav');
const siteHeader = document.querySelector('[data-header]');

if (siteHeader) {
  const updateHeader = () => {
    siteHeader.classList.toggle('is-scrolled', window.scrollY > 12);
  };

  updateHeader();
  window.addEventListener('scroll', updateHeader, { passive: true });
}

if (menuButton && siteNav) {
  const closeMenu = () => {
    menuButton.setAttribute('aria-expanded', 'false');
    document.body.classList.remove('menu-open');
  };

  menuButton.addEventListener('click', () => {
    const isOpen = menuButton.getAttribute('aria-expanded') === 'true';
    menuButton.setAttribute('aria-expanded', String(!isOpen));
    document.body.classList.toggle('menu-open', !isOpen);
  });

  siteNav.addEventListener('click', (event) => {
    if (event.target.closest('a')) closeMenu();
  });

  window.addEventListener('keydown', (event) => {
    if (event.key === 'Escape') closeMenu();
  });

  window.addEventListener('resize', () => {
    if (window.innerWidth > 900) closeMenu();
  });
}

document.querySelectorAll('[data-placeholder-link]').forEach((link) => {
  link.addEventListener('click', (event) => event.preventDefault());
});

const contactForm = document.querySelector('[data-contact-form]');

if (contactForm) {
  contactForm.addEventListener('submit', (event) => event.preventDefault());
}

const projectSlider = document.querySelector('#project-slider');

if (projectSlider) {
  const projectSlides = [
    {
      label: 'Featured Work',
      title: 'NexNovaCo',
      description: 'A modern digital experience for a growing technology brand, designed to present services clearly and communicate innovation with confidence.',
      cta: 'View Project',
      url: 'https://nexnovaco.online-pro.org/',
      mode: 'overview',
      frameLabel: 'NexNovaCo / Homepage',
      caption: 'NexNovaCo desktop homepage overview'
    },
    {
      label: 'NexNovaCo / Full Experience',
      title: 'A Complete Digital Presence',
      description: 'From service storytelling to project highlights, team presentation and brand communication, NexNovaCo was designed as a complete multi-section website experience.',
      cta: 'Explore the Experience',
      url: 'https://nexnovaco.online-pro.org/',
      mode: 'experience',
      frameLabel: 'NexNovaCo / Full Website',
      caption: 'Three views from the complete NexNovaCo website experience'
    },
    {
      label: 'NexNovaCo / Responsive',
      title: 'Responsive by Design',
      description: 'Built to remain clear, fast and visually consistent across mobile and desktop, ensuring a strong experience on every screen size.',
      cta: 'See Mobile View',
      url: 'https://nexnovaco.online-pro.org/',
      mode: 'responsive',
      frameLabel: 'NexNovaCo / Mobile',
      caption: 'NexNovaCo mobile homepage presentation'
    }
  ];

  const previousButton = projectSlider.querySelector('[data-project-prev]');
  const nextButton = projectSlider.querySelector('[data-project-next]');
  const label = projectSlider.querySelector('#project-label');
  const title = projectSlider.querySelector('#work-title');
  const description = projectSlider.querySelector('#project-description');
  const cta = projectSlider.querySelector('#project-cta');
  const ctaText = cta.querySelector('span:first-child');
  const current = projectSlider.querySelector('#project-current');
  const stage = projectSlider.querySelector('.project-stage');
  const frameLabel = projectSlider.querySelector('#project-frame-label');
  const caption = projectSlider.querySelector('#project-visual-caption');
  const artwork = projectSlider.querySelectorAll('[data-project-art]');
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
  let activeIndex = 0;
  let transitionTimer;

  const applySlide = (index) => {
    const slide = projectSlides[index];
    label.textContent = slide.label;
    title.textContent = slide.title;
    description.textContent = slide.description;
    ctaText.textContent = slide.cta;
    cta.href = slide.url;
    cta.target = '_blank';
    cta.rel = 'noopener noreferrer';
    current.textContent = String(index + 1).padStart(2, '0');
    stage.dataset.projectMode = slide.mode;
    frameLabel.textContent = slide.frameLabel;
    caption.textContent = slide.caption;

    artwork.forEach((item) => {
      item.setAttribute('aria-hidden', String(item.dataset.projectArt !== slide.mode));
    });
  };

  const showSlide = (index, animate = true) => {
    activeIndex = (index + projectSlides.length) % projectSlides.length;
    window.clearTimeout(transitionTimer);

    if (animate && !reduceMotion.matches) {
      projectSlider.classList.add('is-changing');
      transitionTimer = window.setTimeout(() => {
        applySlide(activeIndex);
        window.requestAnimationFrame(() => projectSlider.classList.remove('is-changing'));
      }, 180);
      return;
    }

    applySlide(activeIndex);
    projectSlider.classList.remove('is-changing');
  };

  previousButton.addEventListener('click', () => showSlide(activeIndex - 1));
  nextButton.addEventListener('click', () => showSlide(activeIndex + 1));

  projectSlider.addEventListener('keydown', (event) => {
    if (event.key === 'ArrowLeft') {
      event.preventDefault();
      showSlide(activeIndex - 1);
    }

    if (event.key === 'ArrowRight') {
      event.preventDefault();
      showSlide(activeIndex + 1);
    }
  });

  applySlide(activeIndex);
}

const revealItems = document.querySelectorAll('.reveal');

if ('IntersectionObserver' in window) {
  const revealObserver = new IntersectionObserver((entries, observer) => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        entry.target.classList.add('is-visible');
        observer.unobserve(entry.target);
      }
    });
  }, { rootMargin: '0px 0px -8% 0px', threshold: 0.08 });

  revealItems.forEach((item) => revealObserver.observe(item));
} else {
  revealItems.forEach((item) => item.classList.add('is-visible'));
}
