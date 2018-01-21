# Mathematics through LaTeX

Kava Docs supports LaTeX Math markup. For specifics on this standard, see https://en.wikibooks.org/wiki/LaTeX/Mathematics

> Note: To use LaTeX Meth, the feature has to be enabled in the TOC, either globally, or on a topic-by-topic basis, by setting the **useMathematics** to *true*. This is done to prevent performance problems in repositories and/or topics that do not need this feature. (Note that loading all the features required for LaTeX Math features can be very resource intensive, therefore it should only be activated when it is really used). For more information, see [Table of Contents File Structure](TOC File Structure)

For more information, see [Table of Contents File Structure](TOC File Structure) yada yada

## Example 1

```txt
$$
\int^1_\kappa
\left[\bigl(1-w^2\bigr)\bigl(\kappa^2-w^2\bigr)\right]^{-1/2} dw
= \frac{4}{\left(1+\sqrt{\kappa}\,\right)^2} K
\left(\left(\frac{1-\sqrt{\kappa}}{1+\sqrt{\kappa}}\right)^{\!\!2}\right)
$$
```

$$
\int^1_\kappa
\left[\bigl(1-w^2\bigr)\bigl(\kappa^2-w^2\bigr)\right]^{-1/2} dw
= \frac{4}{\left(1+\sqrt{\kappa}\,\right)^2} K
\left(\left(\frac{1-\sqrt{\kappa}}{1+\sqrt{\kappa}}\right)^{\!\!2}\right)
$$

## Example 2

```txt
$$
\mathop{\rm grd} \phi(z) =
\left(a+\frac{2d}{\pi}\right) v_\infty\, \overline{f'(z)} =
v_\infty \left[ \pi a + \frac{2d}{\pi a + 2dw^{-1/2}(w-1)^{1/2}} \right]^-
$$
```

$$
\mathop{\rm grd} \phi(z) =
\left(a+\frac{2d}{\pi}\right) v_\infty\, \overline{f'(z)} =
v_\infty \left[ \pi a + \frac{2d}{\pi a + 2dw^{-1/2}(w-1)^{1/2}} \right]^-
$$

## Example 3

```txt
$$
-\sum^n_{m=1}
\left(\,\sum^\infty_{k=1} \frac{ h^{k-1} }{\left(w_m-z_0\right)^2}
\right) = \sum^\infty_{k=1} s_k\, h^{k-1}
$$
```

$$
-\sum^n_{m=1}
\left(\,\sum^\infty_{k=1} \frac{ h^{k-1} }{\left(w_m-z_0\right)^2}
\right) = \sum^\infty_{k=1} s_k\, h^{k-1}
$$
