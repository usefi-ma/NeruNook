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
  const featuredProjects = [
    {
      name: 'Dr. Emily Parker',
      category: 'Physiotherapy Website — Fictional Demo Concept',
      url: 'https://clinic.online-pro.org/',
      isDemo: true,
      visuals: {
        overview: {
          src: '/assets/clinic/clinic-home-desktop.png',
          alt: 'Dr. Emily Parker fictional physiotherapy demo homepage on desktop',
          position: 'left top'
        },
        experience: {
          layout: 'clinic-pages',
          images: [
            {
              src: '/assets/clinic/about-page.png',
              alt: 'Full About page from the Dr. Emily Parker fictional physiotherapy demo',
              position: 'center top'
            },
            {
              src: '/assets/clinic/Services-page.png',
              alt: 'Full Services page from the Dr. Emily Parker fictional physiotherapy demo',
              position: 'center top'
            },
            {
              src: '/assets/clinic/Videos-page.png',
              alt: 'Full Educational Videos page from the Dr. Emily Parker fictional physiotherapy demo',
              position: 'center top'
            }
          ]
        },
        responsive: {
          desktopSrc: '/assets/clinic/clinic-home-desktop.png',
          desktopAlt: 'Dr. Emily Parker fictional demo desktop homepage',
          desktopPosition: 'center top',
          mobileSrc: '/assets/clinic/clinic-home-mobile.png',
          mobileAlt: 'Dr. Emily Parker fictional demo mobile homepage'
        }
      },
      slides: [
        {
          label: 'Physiotherapy Website — Fictional Demo Concept',
          title: 'Dr. Emily Parker',
          description: 'A modern, calming physiotherapy website concept featuring custom curved layouts, responsive design, service discovery, and an educational video experience.',
          cta: 'View Demo',
          mode: 'overview',
          frameLabel: 'Dr. Emily Parker / Overview',
          caption: 'Fictional physiotherapy website demo — desktop homepage overview'
        },
        {
          label: 'Dr. Emily Parker / Experience · Fictional Demo',
          title: 'A Calm, Guided Experience',
          description: 'About storytelling, clear service discovery and educational video content shape a complete, reassuring journey for prospective patients.',
          cta: 'Explore the Demo',
          mode: 'experience',
          frameLabel: 'Dr. Emily Parker / About, Services & Videos',
          caption: 'About, service discovery and educational video views from the fictional demo'
        },
        {
          label: 'Dr. Emily Parker / Responsive · Fictional Demo',
          title: 'Responsive by Design',
          description: 'Desktop and mobile layouts preserve the curved visual language, readable content and calm experience across screen sizes.',
          cta: 'See Responsive Demo',
          mode: 'responsive',
          frameLabel: 'Dr. Emily Parker / Desktop & Mobile',
          caption: 'Fictional physiotherapy demo shown across desktop and mobile'
        }
      ]
    },
    {
      name: 'NexNovaCo',
      category: 'Technology Website',
      url: 'https://nexnovaco.online-pro.org/',
      isDemo: false,
      visuals: {
        overview: {
          src: '/assets/nexnovaco/Screenshot%202026-10-02%20183632.png',
          alt: 'NexNovaCo desktop homepage overview',
          position: 'center top'
        },
        experience: {
          layout: 'full-page',
          images: [
            {
              src: '/assets/nexnovaco/full%20page.png',
              alt: 'Top of the complete NexNovaCo website',
              position: 'center top'
            },
            {
              src: '/assets/nexnovaco/full%20page.png',
              alt: 'Middle of the complete NexNovaCo website',
              position: 'center 48%'
            },
            {
              src: '/assets/nexnovaco/full%20page.png',
              alt: 'Bottom of the complete NexNovaCo website',
              position: 'center bottom'
            }
          ]
        },
        responsive: {
          desktopSrc: '/assets/nexnovaco/Screenshot%202026-10-02%20183632.png',
          desktopAlt: 'NexNovaCo desktop homepage backdrop',
          desktopPosition: 'center top',
          mobileSrc: '/assets/nexnovaco/mobilr.png',
          mobileAlt: 'NexNovaCo mobile homepage presentation'
        }
      },
      slides: [
        {
          label: 'Featured Work',
          title: 'NexNovaCo',
          description: 'A modern digital experience for a growing technology brand, designed to present services clearly and communicate innovation with confidence.',
          cta: 'View Project',
          mode: 'overview',
          frameLabel: 'NexNovaCo / Homepage',
          caption: 'NexNovaCo desktop homepage overview'
        },
        {
          label: 'NexNovaCo / Full Experience',
          title: 'A Complete Digital Presence',
          description: 'From service storytelling to project highlights, team presentation and brand communication, NexNovaCo was designed as a complete multi-section website experience.',
          cta: 'Explore the Experience',
          mode: 'experience',
          frameLabel: 'NexNovaCo / Full Website',
          caption: 'Three views from the complete NexNovaCo website experience'
        },
        {
          label: 'NexNovaCo / Responsive',
          title: 'Responsive by Design',
          description: 'Built to remain clear, fast and visually consistent across mobile and desktop, ensuring a strong experience on every screen size.',
          cta: 'See Mobile View',
          mode: 'responsive',
          frameLabel: 'NexNovaCo / Mobile',
          caption: 'NexNovaCo mobile homepage presentation'
        }
      ]
    }
  ];

  const projectSlides = featuredProjects.flatMap((project, projectIndex) =>
    project.slides.map((slide, presentationIndex) => ({
      ...slide,
      project,
      projectIndex,
      presentationIndex
    }))
  );

  const previousButton = projectSlider.querySelector('[data-project-prev]');
  const nextButton = projectSlider.querySelector('[data-project-next]');
  const label = projectSlider.querySelector('#project-label');
  const title = projectSlider.querySelector('#work-title');
  const description = projectSlider.querySelector('#project-description');
  const cta = projectSlider.querySelector('#project-cta');
  const ctaText = cta.querySelector('span:first-child');
  const current = projectSlider.querySelector('#project-current');
  const total = projectSlider.querySelector('#project-total');
  const stage = projectSlider.querySelector('.project-stage');
  const frameLabel = projectSlider.querySelector('#project-frame-label');
  const caption = projectSlider.querySelector('#project-visual-caption');
  const artwork = projectSlider.querySelectorAll('[data-project-art]');
  const overviewImage = projectSlider.querySelector('[data-project-overview-image]');
  const experiencePanels = projectSlider.querySelectorAll('[data-project-experience-panel]');
  const responsiveDesktop = projectSlider.querySelector('[data-project-responsive-desktop]');
  const responsiveMobile = projectSlider.querySelector('[data-project-responsive-mobile]');
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
  let activeIndex = 0;
  let transitionTimer;

  total.textContent = String(projectSlides.length).padStart(2, '0');

  const applyProjectVisuals = (project) => {
    const { visuals } = project;
    overviewImage.src = visuals.overview.src;
    overviewImage.alt = visuals.overview.alt;
    overviewImage.style.objectPosition = visuals.overview.position;

    stage.dataset.projectExperience = visuals.experience.layout;
    experiencePanels.forEach((panel, panelIndex) => {
      const image = visuals.experience.images[panelIndex];
      panel.style.backgroundImage = `url("${image.src}")`;
      panel.style.backgroundPosition = image.position;
      panel.setAttribute('aria-label', image.alt);
    });

    responsiveDesktop.src = visuals.responsive.desktopSrc;
    responsiveDesktop.alt = visuals.responsive.desktopAlt;
    responsiveDesktop.style.objectPosition = visuals.responsive.desktopPosition;
    responsiveMobile.src = visuals.responsive.mobileSrc;
    responsiveMobile.alt = visuals.responsive.mobileAlt;
  };

  const applySlide = (index) => {
    const slide = projectSlides[index];
    const { project } = slide;
    label.textContent = slide.label;
    title.textContent = slide.title;
    description.textContent = slide.description;
    ctaText.textContent = slide.cta;
    cta.href = project.url;
    cta.target = '_blank';
    cta.rel = 'noopener noreferrer';
    cta.setAttribute('aria-label', `${slide.cta}: ${project.name}${project.isDemo ? ' fictional demo' : ''} (opens in a new tab)`);
    current.textContent = String(index + 1).padStart(2, '0');
    stage.dataset.projectMode = slide.mode;
    frameLabel.textContent = slide.frameLabel;
    caption.textContent = slide.caption;
    projectSlider.setAttribute(
      'aria-label',
      `Featured work: ${project.name}${project.isDemo ? ' fictional demo concept' : ''}, project ${slide.projectIndex + 1} of ${featuredProjects.length}, presentation ${slide.presentationIndex + 1} of ${project.slides.length}`
    );

    applyProjectVisuals(project);

    artwork.forEach((item) => {
      item.setAttribute('aria-hidden', String(item.dataset.projectArt !== slide.mode));
    });

    const previousSlide = projectSlides[(index - 1 + projectSlides.length) % projectSlides.length];
    const nextSlide = projectSlides[(index + 1) % projectSlides.length];
    previousButton.setAttribute('aria-label', `Previous: ${previousSlide.project.name} — ${previousSlide.frameLabel.split(' / ').pop()}`);
    nextButton.setAttribute('aria-label', `Next: ${nextSlide.project.name} — ${nextSlide.frameLabel.split(' / ').pop()}`);
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
