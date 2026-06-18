---
name: Peak Expedition
colors:
  surface: '#f8f9fa'
  surface-dim: '#d9dadb'
  surface-bright: '#f8f9fa'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f3f4f5'
  surface-container: '#edeeef'
  surface-container-high: '#e7e8e9'
  surface-container-highest: '#e1e3e4'
  on-surface: '#191c1d'
  on-surface-variant: '#594139'
  inverse-surface: '#2e3132'
  inverse-on-surface: '#f0f1f2'
  outline: '#8d7168'
  outline-variant: '#e1bfb5'
  surface-tint: '#ab3500'
  primary: '#ab3500'
  on-primary: '#ffffff'
  primary-container: '#ff6b35'
  on-primary-container: '#5f1900'
  inverse-primary: '#ffb59d'
  secondary: '#3f6653'
  on-secondary: '#ffffff'
  secondary-container: '#beead1'
  on-secondary-container: '#436b58'
  tertiary: '#0e6c4a'
  on-tertiary: '#ffffff'
  tertiary-container: '#57a982'
  on-tertiary-container: '#003a25'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdbd0'
  primary-fixed-dim: '#ffb59d'
  on-primary-fixed: '#390c00'
  on-primary-fixed-variant: '#832600'
  secondary-fixed: '#c1ecd4'
  secondary-fixed-dim: '#a5d0b9'
  on-secondary-fixed: '#002114'
  on-secondary-fixed-variant: '#274e3d'
  tertiary-fixed: '#a0f4c8'
  tertiary-fixed-dim: '#85d7ad'
  on-tertiary-fixed: '#002113'
  on-tertiary-fixed-variant: '#005236'
  background: '#f8f9fa'
  on-background: '#191c1d'
  surface-variant: '#e1e3e4'
typography:
  display-lg:
    fontFamily: Outfit
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  display-lg-mobile:
    fontFamily: Outfit
    fontSize: 36px
    fontWeight: '700'
    lineHeight: 44px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Outfit
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
  headline-md:
    fontFamily: Outfit
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Outfit
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Outfit
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-md:
    fontFamily: Outfit
    fontSize: 14px
    fontWeight: '500'
    lineHeight: 20px
    letterSpacing: 0.01em
  label-sm:
    fontFamily: Outfit
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 8px
  gutter: 16px
  margin-mobile: 20px
  margin-desktop: 40px
  container-max: 1280px
---

## Brand & Style
The design system is crafted for an adventurous yet accessible outdoor experience. It balances the rugged nature of exploration with a friendly, welcoming interface that encourages users of all skill levels to discover new heights. 

The aesthetic follows a **Modern** movement with a touch of **Tactile** warmth. It prioritizes clarity and high-quality photography, using generous whitespace to allow content to breathe. The emotional response should be one of optimism, reliability, and excitement. It avoids the harshness of traditional "survivalist" gear in favor of a contemporary "lifestyle adventurer" look—clean lines, soft edges, and a vibrant, energetic spirit.

## Colors
The palette is inspired by the transition from basecamp to summit.

- **Primary (Adventure Orange):** Used for primary actions, calls to action, and highlights. It provides high visibility and energy.
- **Secondary (Forest Deep):** A sophisticated dark green used for navigation bars, heavy text, and grounded elements.
- **Tertiary (Mountain Meadow):** A soft, natural green used for success states, badges, and secondary accents.
- **Neutral:** A range of warm grays and off-whites that prevent the UI from feeling sterile, providing a soft canvas for the bolder brand colors.

Surface colors should use subtle shifts in value to define hierarchy rather than heavy lines.

## Typography
This design system utilizes **Outfit** across all levels to maintain a cohesive, friendly, and modern geometric appearance. Its rounded terminals and open counters ensure legibility while reinforcing the approachable brand personality.

- **Display & Headlines:** Use tighter letter spacing and heavier weights to create a strong visual anchor.
- **Body Text:** Use regular weights with comfortable line heights to ensure long-form content is easy to digest during planning phases.
- **Labels:** Use medium or semi-bold weights. Small labels should utilize slight tracking and uppercase styling to distinguish them from body copy.

## Layout & Spacing
The layout follows a **Fluid Grid** model based on an 8px square system. This ensures mathematical harmony between all elements.

- **Grid:** A 12-column grid for desktop and a 4-column grid for mobile.
- **Rhythm:** Vertical spacing should favor generous gaps (32px, 48px, or 64px) between major sections to prevent the UI from feeling cluttered.
- **Touch Targets:** Minimum interactive areas should be 44x44px to accommodate outdoor usage where precision may be lower.

## Elevation & Depth
Hierarchy is conveyed through **Tonal Layers** and **Ambient Shadows**. 

The design avoids harsh blacks for shadows, instead using low-opacity tints of the Secondary color (Forest Deep) to create a more natural, outdoor-inspired depth.
- **Level 0 (Surface):** The base background color.
- **Level 1 (Card):** Slight elevation using a very soft, diffused shadow (15% opacity) to suggest interactability.
- **Level 2 (Dropdowns/Modals):** Higher elevation with a larger blur radius to pull the element forward.
- **Level 3 (Sticky Nav):** Uses a subtle backdrop blur (glassmorphism) when scrolling over content to maintain context without losing legibility.

## Shapes
The shape language is **Rounded**, mirroring the friendly nature of the typography. 

- **Components:** Standard buttons and input fields use a 0.5rem (8px) corner radius.
- **Cards:** Larger containers and image wrappers use "rounded-lg" (1rem / 16px) to create a soft, modern frame for photography.
- **Chips:** Tags and status indicators use a "pill" shape (full rounding) to differentiate them from actionable buttons.

## Components
- **Buttons:** Primary buttons feature a solid Adventure Orange fill with white text. Secondary buttons use a thick 2px outline of Forest Deep. Hover states should involve a subtle scale-up (1.02x) rather than just a color change, leaning into the tactile feel.
- **Input Fields:** Use a light gray background with a subtle bottom border that transforms into a full primary-colored outline upon focus.
- **Cards:** Content cards should have a subtle 1px border in a light neutral shade to define boundaries on white backgrounds, coupled with a soft shadow.
- **Chips:** Small, pill-shaped elements used for difficulty levels (e.g., "Easy", "Moderate", "Hard") with background colors that correspond to the difficulty.
- **Lists:** Use generous vertical padding (16px) between items with thin, 1px dividers that don't reach the full width of the container.
- **Checkboxes/Radios:** Large, easily tappable areas that use the primary color for the "checked" state, maintaining the rounded shape language (even for checkboxes, use a slight 4px radius).