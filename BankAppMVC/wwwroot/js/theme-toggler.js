﻿
        /*!
        * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
        * Copyright 2011-2024 The Bootstrap Authors
        * Licensed under the Creative Commons Attribution 3.0 Unported License.
        * Modified by Simpleqode
        */

        (() => {
            'use strict';

          const getStoredTheme = () => localStorage.getItem('theme');
          const setStoredTheme = (theme) => localStorage.setItem('theme', theme);

          const getStoredNavigationPosition = () => localStorage.getItem('navigationPosition');
          const setStoredNavigationPosition = (navigationPosition) => localStorage.setItem('navigationPosition', navigationPosition);

          const getPreferredTheme = () => {
            const storedTheme = getStoredTheme();
        if (storedTheme) {
              return storedTheme;
            }
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
          };

          const getPreferredNavigationPosition = () => {
            const storedNavigationPosition = getStoredNavigationPosition();
        if (storedNavigationPosition) {
              return storedNavigationPosition;
            }
        return 'sidenav';
          };

          const setTheme = (theme) => {
            if (theme === 'auto') {
            document.documentElement.setAttribute('data-bs-theme', window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
            } else {
            document.documentElement.setAttribute('data-bs-theme', theme);
            }
          };

          const setNavigationPosition = (navigationPosition) => {
            document.documentElement.setAttribute('data-bs-navigation-position', navigationPosition);
          };

        setTheme(getPreferredTheme());
        setNavigationPosition(getPreferredNavigationPosition());

          const showActiveTheme = (theme, settingsSwitcher) => {
            document.querySelectorAll('[data-bs-theme-value]').forEach((element) => {
                element.classList.remove('active');
                element.setAttribute('aria-pressed', 'false');

                if (element.getAttribute('data-bs-theme-value') === theme) {
                    element.classList.add('active');
                    element.setAttribute('aria-pressed', 'true');
                }
            });
        if (settingsSwitcher) {
            settingsSwitcher.focus();
            }
          };

          const showActiveNavigationPosition = (navigationPosition, settingsSwitcher) => {
            document.querySelectorAll('[data-bs-navigation-position-value]').forEach((element) => {
                element.classList.remove('active');
                element.setAttribute('aria-pressed', 'false');

                if (element.getAttribute('data-bs-navigation-position-value') === navigationPosition) {
                    element.classList.add('active');
                    element.setAttribute('aria-pressed', 'true');
                }
            });

        if (settingsSwitcher) {
            settingsSwitcher.focus();
            }
          };

          const refreshCharts = () => {
            const charts = document.querySelectorAll('.chart-canvas');

            charts.forEach((chart) => {
              const chartId = chart.getAttribute('id');
        const instance = Chart.getChart(chartId);

        if (!instance) {
                return;
              }

        if (instance.options.scales.y) {
            instance.options.scales.y.grid.color = getComputedStyle(document.documentElement).getPropertyValue('--bs-border-color');
        instance.options.scales.y.ticks.color = getComputedStyle(document.documentElement).getPropertyValue('--bs-secondary-color');
              }

        if (instance.options.scales.x) {
            instance.options.scales.x.ticks.color = getComputedStyle(document.documentElement).getPropertyValue('--bs-secondary-color');
              }

        if (instance.options.elements.arc) {
            instance.options.elements.arc.borderColor = getComputedStyle(document.documentElement).getPropertyValue('--bs-body-bg');
        instance.options.elements.arc.hoverBorderColor = getComputedStyle(document.documentElement).getPropertyValue('--bs-body-bg');
              }

        instance.update();
            });
          };

          window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
            const storedTheme = getStoredTheme();
        if (storedTheme !== 'light' && storedTheme !== 'dark') {
            setTheme(getPreferredTheme());
            }
          });

          window.addEventListener('DOMContentLoaded', () => {
            showActiveTheme(getPreferredTheme());
        showActiveNavigationPosition(getPreferredNavigationPosition());

            document.querySelectorAll('[data-bs-theme-value]').forEach((toggle) => {
            toggle.addEventListener('click', (e) => {
                e.preventDefault();
                const theme = toggle.getAttribute('data-bs-theme-value');
                const settingsSwitcher = toggle.closest('.nav-item').querySelector('[data-bs-settings-switcher]');
                console.log(settingsSwitcher);
                setStoredTheme(theme);
                setTheme(theme);
                showActiveTheme(theme, settingsSwitcher);
                refreshCharts();
            });
            });

            document.querySelectorAll('[data-bs-navigation-position-value]').forEach((toggle) => {
            toggle.addEventListener('click', (e) => {
                e.preventDefault();
                const navigationPosition = toggle.getAttribute('data-bs-navigation-position-value');
                const settingsSwitcher = toggle.closest('.nav-item').querySelector('[data-bs-settings-switcher]');
                setStoredNavigationPosition(navigationPosition);
                setNavigationPosition(navigationPosition);
                showActiveNavigationPosition(navigationPosition, settingsSwitcher);
            });
            });
          });
        })();

